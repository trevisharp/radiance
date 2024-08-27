/* Author:  Leonardo Trevisan Silio
 * Date:    27/08/2024
 */
using System;
using System.Text;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace Radiance.Renders;

using Data;
using Shaders;
using Shaders.Objects;

/// <summary>
/// A global thread-safe context to render execution.
/// </summary>
public static class GlobalRenderContext
{
    private static readonly Dictionary<int, RenderContext> threadMap = [];

    /// <summary>
    /// Create a new RenderContext for this thread.
    /// </summary>
    public static RenderContext CreateContext()
    {
        var id = GetCurrentThreadId();
        threadMap.Remove(id);

        var ctx = new RenderContext
        {
            Position = new("pos", ShaderOrigin.VertexShader, [ Utils.bufferDep ]),
            Color = new("vec4(0.0, 0.0, 0.0, 1.0)", ShaderOrigin.FragmentShader, [])
        };
        threadMap.Add(id, ctx);
        return ctx;
    }

    /// <summary>
    /// Remove the RenderContext for this thread.
    /// </summary>
    public static void ClearContext()
    {
        var id = GetCurrentThreadId();
        threadMap.Remove(id);
    }

    /// <summary>
    /// Get the RenderContext from thread if exists. Otherwise recive null.
    /// </summary>
    public static RenderContext GetContext()
    {
        var id = GetCurrentThreadId();
        return threadMap.TryGetValue(id, out RenderContext value) ? value : null;
    }
    
    private static int GetCurrentThreadId()
    {
        var crr = Thread.CurrentThread;
        var id  = crr.ManagedThreadId;
        return id;
    }
}