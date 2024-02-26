/* Author:  Leonardo Trevisan Silio
 * Date:    26/02/2024
 */
using System.Threading;
using System.Collections.Generic;

namespace Radiance.Pipelines;

using Data;
using Renders;

/// <summary>
/// Represents a thread-safe pipeline of many renders drawing objects.
/// </summary>
public class PipelineContext
{
    private static Dictionary<int, PipelineContext> threadMap = new();

    public static PipelineContext CreateContext()
    {
        var crr = Thread.CurrentThread;
        var id  = crr.ManagedThreadId;
        deleteContextIfExist(id);

        var ctx = new PipelineContext{};
        threadMap.Add(id, ctx);
        return ctx;
    }

    public static void ClearContext()
    {
        var crr = Thread.CurrentThread;
        var id  = crr.ManagedThreadId;
        deleteContextIfExist(id);
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
        
    }

    public void RegisterRenderCall(RenderContext render, Polygon poly, object[] data)
        => renders.Add(new (render, poly, data));

    private record RenderInfo(
        RenderContext Render,
        Polygon Polygon,
        object[] Parameters
    );
}