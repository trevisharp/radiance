/* Author:  Leonardo Trevisan Silio
 * Date:    27/02/2024
 */
using System;
using System.Threading;
using System.Collections.Generic;

namespace Radiance.Pipelines;

using Data;
using Renders;

/// <summary>
/// Represents a thread-safe pipeline of many renders drawing objects.
/// </summary>
public class PipelineContext(Action pipelineFunction)
{
    private static readonly Dictionary<int, PipelineContext> threadMap = [];
    
    internal static int GetCurrentThreadId()
    {
        var crr = Thread.CurrentThread;
        var id  = crr.ManagedThreadId;
        return id;
    }

    public static void SetContext(PipelineContext context)
    {
        var id = GetCurrentThreadId();
        threadMap.Remove(id);
        threadMap.Add(id, context);
    }

    public static PipelineContext GetContext()
    {
        var id  = GetCurrentThreadId();
        return threadMap.TryGetValue(id, out PipelineContext value) ? value : null;
    }

    private readonly List<RenderInfo> renders = [];

    public void Render()
    {
        SetContext(this);
        pipelineFunction();
    }

    public void RegisterRenderCall(RenderContext render, Polygon poly, object[] data)
        => renders.Add(new (render, poly, data));

    record RenderInfo(
        RenderContext Render,
        Polygon Polygon,
        object[] Parameters
    );
}