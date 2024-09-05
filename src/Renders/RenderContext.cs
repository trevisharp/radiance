/* Author:  Leonardo Trevisan Silio
 * Date:    04/09/2024
 */
using System.Threading;
using System.Collections.Generic;

namespace Radiance.Renders;

using System;
using Radiance.Primitives;
using Shaders;
using Shaders.Objects;

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

    public record RenderCall(
        Render Render,
        object[] Arguments
    );

    public Vec3ShaderObject Position { get; set; } = new("pos", ShaderOrigin.VertexShader, [ Utils.bufferDep ]);

    public Vec4ShaderObject Color { get; set; } = new("vec4(0.0, 0.0, 0.0, 1.0)", ShaderOrigin.FragmentShader, []);

    public List<object> CallHistory { get; private set; } = [];

    public void RegisterCall(Render render, object[] arguments)
        => CallHistory.Add(new RenderCall(render, arguments));

    public void AddClear(Vec4 color)
    {
        throw new NotImplementedException();
    }

    public void AddDraw()
    {
        throw new NotImplementedException();
    }

    public void AddFill()
    {
        throw new NotImplementedException();
    }

    public void AddStrip()
    {
        throw new NotImplementedException();
    }

    public void AddFan()
    {
        throw new NotImplementedException();
    }

    public void AddLines()
    {
        throw new NotImplementedException();
    }
}