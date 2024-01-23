/* Author:  Leonardo Trevisan Silio
 * Date:    23/01/2024
 */
using System.Linq;
using System.Dynamic;
using System.Collections.Generic;
using System;
using Radiance.Exceptions;
using System.ComponentModel;

namespace Radiance.Renders;

/// <summary>
/// Represents a Render with some parameters gived resulting in a
/// function with less parameters that the original Render.
/// </summary>
public class CurryingRender : DynamicObject, ICurryable
{
    private Render parent;
    private List<object> parameters;

    public Render Parent => parent;
    public IEnumerable<object> GivedParameters => parameters;

    public CurryingRender(Render parent, params object[] objects)
    {
        this.parent = parent;
        this.parameters = new();
        this.parameters.AddRange(objects);
    }

    public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
    {
        var totalParams = GivedParameters
            .Concat(args)
            .ToArray();
        
        parent.TryInvoke(binder, totalParams, out result);
        return true;
    }

    public dynamic Curry(params object[] parameters)
        => new CurryingRender(parent, this.parameters.Concat(parameters).ToArray());
    
    public static implicit operator Action(CurryingRender render)
    {
        var expectedParams = render.parent.ExtraParameterCount + 1;
        var actualParams = render.parameters.Count;
        if (expectedParams < actualParams)
            throw new SurplusParametersException();
        
        if (expectedParams > actualParams)
            throw new MissingParametersException();
        
        return () => render.parent.TryInvoke(
            null, render.parameters.ToArray(), out _);
    }
}