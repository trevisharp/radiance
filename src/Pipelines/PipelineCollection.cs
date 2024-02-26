/* Author:  Leonardo Trevisan Silio
 * Date:    26/02/2024
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace Radiance.Pipelines;

/// <summary>
/// Represents a collection of pipelines.
/// </summary>
public class PipelineCollection : IEnumerable<PipelineContext>
{
    private List<PipelineContext> pipelines = new();

    public PipelineCollection Add(PipelineContext ctx)
    {
        pipelines.Add(ctx);
        return this;
    }

    public void Render()
    {
        foreach (var pipeline in pipelines)
            pipeline.Render();
    }

    public IEnumerator<PipelineContext> GetEnumerator()
        => pipelines.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
    
    public static implicit operator PipelineCollection(
        PipelineContext ctx
    ) => [ctx];

    public static PipelineCollection operator +(
        PipelineCollection coll, PipelineContext ctx
    ) => coll.Add(ctx);

    public static PipelineCollection operator +(
        PipelineCollection coll, Action renderCode
    )
    {
        var newPipeline = PipelineContext.CreateContext();
        renderCode();
        PipelineContext.ClearContext();
        return coll + newPipeline;
    }
}