/* Author:  Leonardo Trevisan Silio
 * Date:    04/09/2024
 */
using System;
using System.Threading;
using System.Collections.Generic;

namespace Radiance.Renders;

using OpenGL4;
using Shaders;
using Shaders.Objects;
using Shaders.CodeGeneration;
using Shaders.CodeGeneration.GLSL;
using Managers;
using Primitives;

/// <summary>
/// A Thread-Safe global context data object.
/// </summary>
public class RenderContext
{
    public static ShaderManagerBuilder ShaderContextBuilder { get; set; } = new OpenGL4ShaderManagerBuilder();
    public static ProgramManagerBuilder ProgramContextBuilder { get; set; } = new OpenGL4ProgramManagerBuilder();
    public static ICodeGeneratorBuilder CodeGeneratorBuilder { get; set; } = new GLSLGeneratorBuilder();
    
    static readonly Dictionary<int, RenderContext> threadMap = [];

    static int GetCurrentThreadId()
    {
        var crr = Thread.CurrentThread;
        var id  = crr.ManagedThreadId;
        return id;
    }

    /// <summary>
    /// Open a new context for this thread.
    /// </summary>
    public static RenderContext OpenContext()
    {
        CloseContext();

        var openedContext = new RenderContext();
        var id = GetCurrentThreadId();
        threadMap.Add(id, openedContext);

        return openedContext;
    }

    /// <summary>
    /// Close the context for this thread.
    /// </summary>
    public static void CloseContext()
    {
        var ctx = GetContext();
        if (ctx is null)
            return;

        var id = GetCurrentThreadId();
        threadMap.Remove(id);
    }

    /// <summary>
    /// Get the opened context for this thread or null if it is closed.
    /// </summary>
    public static RenderContext? GetContext()
    {
        var id = GetCurrentThreadId();
        return threadMap.TryGetValue(id, out var ctx)
            ? ctx : null;
    }

    public readonly ProgramManager ProgramContext = ProgramContextBuilder.Build();

    public bool Verbose { get; set; } = false;

    public Action<Polygon, object[]>? RenderActions { get; set; }

    public Vec3ShaderObject Position { get; set; } = new("pos", ShaderOrigin.VertexShader, [ Utils.bufferDep ]);

    public Vec4ShaderObject Color { get; set; } = new("vec4(0.0, 0.0, 0.0, 1.0)", ShaderOrigin.FragmentShader, []);

    public List<object> CallHistory { get; private set; } = [];

    public void AddClear(Vec4 color)
        => RenderActions += (_, _) => ProgramContext.Clear(color);

    public void AddPoints() 
        => AddDrawOperation(PrimitiveType.Points);

    public void AddLines() 
        => AddDrawOperation(PrimitiveType.Lines);
    
    public void AddDraw() 
        => AddDrawOperation(PrimitiveType.LineLoop);
    
    public void AddFill()
        => AddDrawOperation(PrimitiveType.Triangles, true);
    
    public void AddTriangules() 
        => AddDrawOperation(PrimitiveType.Triangles);
    
    public void AddStrip() 
        => AddDrawOperation(PrimitiveType.TriangleStrip);
    
    public void AddFan() 
        => AddDrawOperation(PrimitiveType.TriangleFan);
        
    private void AddDrawOperation(
        PrimitiveType primitive, 
        bool needTriangularization = false
    )
    {
        var shaderManager = ShaderContextBuilder.Build();

        var generator = CodeGeneratorBuilder.Build();
        var pair = generator.GenerateShaders(Position, Color, shaderManager);
        
        RenderActions += (poly, data) =>
        {
            var program = ProgramContext.CreateProgram(pair, Verbose);
            shaderManager.SetProgram(program);

            if (needTriangularization)
                poly = poly.Triangulation;

            shaderManager.CreateResources(poly);
            ProgramContext.UseProgram(program);

            shaderManager.Use(poly);

            if (pair.VertexShader.Setup is not null)
                pair.VertexShader.Setup();

            if (pair.FragmentShader.Setup is not null)
                pair.FragmentShader.Setup();

            shaderManager.Draw(primitive, poly);
        };
    }
}