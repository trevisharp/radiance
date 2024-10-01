/* Author:  Leonardo Trevisan Silio
 * Date:    01/10/2024
 */
using System.Collections.Generic;

namespace Radiance.Ensemble;

using Shaders;
using Exceptions;

/// <summary>
/// Represents a collections of shaders that can be optimized on call action.
/// </summary>
public class RenderCollection<T>(dynamic baseRender, object[] pinedValues)
    where T : ShaderObject
{
    readonly List<T> callings = [];

}

public class RenderCollection<T1, T2>(dynamic baseRender, object[] pinedValues)
    where T1 : ShaderObject
    where T2 : ShaderObject
{
    readonly List<(T1, T2)> callings = [];
    
    public RenderCollection<T1> Pin(T2 argument)
    {
        if (callings.Count > 0)
            throw new UnpinableCollectionException();

        object[] newPinedValues =  [ ..pinedValues, argument ];
        return new RenderCollection<T1>(baseRender, newPinedValues);
    }
}