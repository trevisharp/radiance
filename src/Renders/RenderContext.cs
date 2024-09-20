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
using System.Net.WebSockets;

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

    /// <summary>
    /// Get or set if the context is in verbose mode.
    /// </summary>
    public bool Verbose { get; set; } = false;

    /// <summary>
    /// Get or set the actions in this render context.
    /// </summary>
    public Action<Polygon, object[]>? RenderActions { get; set; }

    /// <summary>
    /// Get or set the shader object representing the position transformation.
    /// </summary>
    public Vec3ShaderObject Position { get; set; } = new("pos", ShaderOrigin.VertexShader, [ ShaderDependence.BufferDep ]);

    /// <summary>
    /// Get or set the shader object representing the color transformation.
    /// </summary>
    public Vec4ShaderObject Color { get; set; } = new("vec4(0.0, 0.0, 0.0, 1.0)", ShaderOrigin.FragmentShader, []);

    /// <summary>
    /// Call render pipeline for this render context.
    /// </summary>
    public void Render(Polygon polygon, object[] arguments)
    {
        if (RenderActions is null)
            return;
        
        RenderActions(polygon, arguments);
    }
    
    /// <summary>
    /// Add a clear opeartion to this render context.
    /// </summary>
    public void AddClear(Vec4 color)
        => RenderActions += (_, _) => ProgramContext.Clear(color);

    /// <summary>
    /// Add a draw points opeartion to this render context.
    /// </summary>
    public void AddPoints() 
        => AddDrawOperation(PrimitiveType.Points);

    /// <summary>
    /// Add a draw lines opeartion to this render context.
    /// </summary>
    public void AddLines() 
        => AddDrawOperation(PrimitiveType.Lines);
    
    /// <summary>
    /// Add a draw line loop to this render context.
    /// </summary>
    public void AddDraw() 
        => AddDrawOperation(PrimitiveType.LineLoop);
    
    /// <summary>
    /// Add a draw triangules opeartion with triangularization to this render context.
    /// </summary>
    public void AddFill()
        => AddDrawOperation(PrimitiveType.Triangles, true);
    
    /// <summary>
    /// Add a draw triangules opeartion to this render context.
    /// </summary>
    public void AddTriangules() 
        => AddDrawOperation(PrimitiveType.Triangles);
    
    /// <summary>
    /// Add a draw triangules strip opeartion to this render context.
    /// </summary>
    public void AddStrip() 
        => AddDrawOperation(PrimitiveType.TriangleStrip);
    
    /// <summary>
    /// Add a draw triangules fan opeartion to this render context.
    /// </summary>
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