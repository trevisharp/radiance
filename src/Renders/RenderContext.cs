/* Author:  Leonardo Trevisan Silio
 * Date:    04/09/2024
 */
using System;
using System.Threading;
using System.Collections.Generic;

namespace Radiance.Renders;

using Shaders;
using Shaders.CodeGen;
using Shaders.Objects;
using Contexts;
using Contexts.OpenGL4;
using Primitives;

/// <summary>
/// A Thread-Safe global context data object.
/// </summary>
public class RenderContext
{
    public static ShaderContextBuilder ShaderContextBuilder { get; set; } = new OpenGL4ShaderContextBuilder();
    public static ProgramContextBuilder ProgramContextBuilder { get; set; } = new OpenGL4ProgramContextBuilder();
    
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
    public static void OpenContext()
    {
        CloseContext();

        var id = GetCurrentThreadId();
        threadMap.Add(id, new());
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

    /// <summary>
    /// Get or Set the GSLS Version. The default value is '330 core'
    /// </summary>
    public string VersionText { get; set; } = "330 core";

    public readonly ProgramContext ProgramContext = ProgramContextBuilder.Build();

    public bool Verbose { get; set; } = false;

    public Action<Polygon, object[]>? DrawOperations { get; set; }

    public Vec3ShaderObject Position { get; set; } = new("pos", ShaderOrigin.VertexShader, [ Utils.bufferDep ]);

    public Vec4ShaderObject Color { get; set; } = new("vec4(0.0, 0.0, 0.0, 1.0)", ShaderOrigin.FragmentShader, []);

    public List<object> CallHistory { get; private set; } = [];

    public void AddClear(Vec4 color)
        => DrawOperations += (_, _) => ProgramContext.Clear(color);

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
        var shaderCtx = ShaderContextBuilder.Build();

        var generator = new GLSLGenerator(VersionText);
        var pair = generator.GenerateShaders(Position, Color, shaderCtx);
        
        DrawOperations += (poly, data) =>
        {
            var program = ProgramContext.CreateProgram(pair, Verbose);

            if (needTriangularization)
                poly = poly.Triangulation;

            shaderCtx.CreateResources(poly);
            ProgramContext.UseProgram(program);

            // shaderCtx.Use(poly);

            // if (vertSetup is not null)
            //     vertSetup();

            // if (fragSetup is not null)
            //     fragSetup();

            // GL.DrawArrays(primitive, 0, poly.Data.Count() / 3);
        };
    }
    
}