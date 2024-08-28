/* Author:  Leonardo Trevisan Silio
 * Date:    27/08/2024
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace Radiance.Pipelines;

/// <summary>
/// Represents a collection of pipelines.
/// </summary>
public class PipelineCollection : IEnumerable<PipelineContext>
{
    private readonly List<PipelineContext> pipelines = [];

    public PipelineCollection Add(PipelineContext ctx)
    {
        pipelines.Add(ctx);
        return this;
    }
    
    public PipelineCollection Remove(PipelineContext ctx)
    {
        pipelines.Remove(ctx);
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

    public static PipelineCollection operator -(
        PipelineCollection coll, PipelineContext ctx
    ) => coll.Remove(ctx);

    public static PipelineCollection operator +(
        PipelineCollection coll, Action pipelineFunction
    ) => coll + new PipelineContext(pipelineFunction);

    public static PipelineCollection operator -(
        PipelineCollection coll, Action pipelineFunction
    ) => coll - new PipelineContext(pipelineFunction);
}