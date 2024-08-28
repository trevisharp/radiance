/* Author:  Leonardo Trevisan Silio
 * Date:    28/08/2024
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

    private List<RenderInfo> renders = null;

    public void Render()
    {
        Load();

        foreach (var render in renders)
        {
            var ctx = render.Render;
            var poly = render.Polygon;
            var parameters = render.Parameters;

            ctx.Render(poly, parameters);
        }
    }

    void Load()
    {
        if (renders is not null)
            return;
        renders = [];

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