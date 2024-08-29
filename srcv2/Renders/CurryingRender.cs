/* Author:  Leonardo Trevisan Silio
 * Date:    19/08/2024
 */
using System;
using System.Linq;
using System.Dynamic;
using System.Collections.Generic;

namespace Radiance.Renders;

using Exceptions;

/// <summary>
/// Represents a Render with some parameters gived resulting in a
/// function with less parameters that the original Render.
/// </summary>
public class CurryingRender(Render parent, params object[] objects) : DynamicObject, ICurryable
{
    private readonly Render parent = parent ?? throw new ArgumentNullException(nameof(parent));
    private readonly List<object> parameters = [ ..objects ];
    private Action actionData;

    public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        => parent.TryInvoke(binder, [ ..parameters, ..args ], out result);

    public dynamic Curry(params object[] parameters)
        => new CurryingRender(parent, this.parameters.Concat(parameters).ToArray());
    
    public static implicit operator Action(CurryingRender render)
    {
        if (render.actionData is not null)
            return render.actionData;
        
        var expectedParams = render.parent.ExtraParameterCount + 1;
        var actualParams = render.parameters.Count;

        if (expectedParams < actualParams)
            throw new SurplusParametersException();
        
        if (expectedParams > actualParams)
            throw new MissingParametersException();
        
        render.actionData = () => render.parent.TryInvoke(
            null, [ ..render.parameters ], out _);
        return render.actionData;
    }
}