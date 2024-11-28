/* Author:  Leonardo Trevisan Silio
 * Date:    28/11/2024
 */
using System;
using System.Threading;
using System.Collections.Generic;

namespace Radiance.Contexts;

using Buffers;
using Shaders;
using Shaders.Objects;
using Exceptions;
using Implementations;

/// <summary>
/// A Thread-Safe global context data object.
/// </summary>
public class RenderContext
{   
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

    /// <summary>
    /// Get or set if the context is in verbose mode.
    /// </summary>
    public bool Verbose { get; set; } = false;

    /// <summary>
    /// Get or set the actions in this render context.
    /// </summary>
    public Action<object[]>? RenderActions { get; set; }

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
    public void Render(object[] args)
    {
        if (RenderActions is null)
            return;
        
        RenderActions(args);
    }

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
    public void AddLineLoop() 
        => AddDrawOperation(PrimitiveType.LineLoop);
    
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
    
    /// <summary>
    /// Add a draw triangules opeartion with triangularization to this render context.
    /// </summary>
    public void AddFill()
        => AddDrawOperation(PrimitiveType.Triangles, true);

    /// <summary>
    /// Add a draw lines operation with 
    /// </summary>
    public void AddDraw()
        => AddDrawOperation(PrimitiveType.Lines, true);
        
    void AddDrawOperation(PrimitiveType primitive, bool decompose = false)
    {
        var context = ImplementationConfig.Implementation.NewContext();

        var generator = ImplementationConfig.Implementation.NewCodeGenerator();
        
        var pair = generator.GenerateShaders(Position, Color, context);

        context.CreateProgram(pair, Verbose);
        context.UseProgram();

        bool alreadyInitied = false;
        void initIfNeeded()
        {
            if (alreadyInitied)
                return;
            alreadyInitied = true;
            
            if (pair.InitialConfiguration is not null)
                pair.InitialConfiguration();

            context.FirstConfiguration();
        }

        void setupShaders()
        {
            if (pair.VertexShader.Setup is not null)
                pair.VertexShader.Setup();

            if (pair.FragmentShader.Setup is not null)
                pair.FragmentShader.Setup();
        }

        RenderActions += args =>
        {
            IBufferedData polygon = (decompose, args[0]) switch
            {
                (false, IPolygon poly) => poly,
                (true, IPolygon poly) => poly.Triangules,
                (false, Vec3Buffer) => throw new InvalidBufferDrawException(),
                (true, Vec3Buffer buffer) => buffer, 
                (true, IBufferedData) => throw new InvalidFillOperationException(),
                _ => throw new MissingPolygonException()
            };
            args = [ polygon, ..args[1..] ];

            initIfNeeded();
            
            context.UseProgram();
            
            context.InitArgs(args);

            context.UseArgs(args);

            setupShaders();

            context.Draw(primitive, polygon);
        };
    }
}