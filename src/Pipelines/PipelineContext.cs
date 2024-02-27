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
    private static Dictionary<int, PipelineContext> threadMap = new();

    public static void SetContext(PipelineContext context)
    {
        var crr = Thread.CurrentThread;
        var id  = crr.ManagedThreadId;
        deleteContextIfExist(id);
        threadMap.Add(id, context);
    }

    public static PipelineContext GetContext()
    {
        var crr = Thread.CurrentThread;
        var id  = crr.ManagedThreadId;
        if (threadMap.ContainsKey(id))
            return threadMap[id];
        
        return null;
    }

    private static void deleteContextIfExist(int id)
    {
        if (threadMap.ContainsKey(id))
            threadMap.Remove(id);
    }

    private List<RenderInfo> renders = new();

    public void Render()
    {
        SetContext(this);
        pipelineFunction();
    }

    public void RegisterRenderCall(RenderContext render, Polygon poly, object[] data)
        => renders.Add(new (render, poly, data));

    private record RenderInfo(
        RenderContext Render,
        Polygon Polygon,
        object[] Parameters
    );
}