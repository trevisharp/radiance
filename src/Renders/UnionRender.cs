/* Author:  Leonardo Trevisan Silio
 * Date:    02/10/2024
 */
using System;
using System.Reflection;

namespace Radiance.Renders;

using Buffers;
using Shaders;

/// <summary>
/// A render that unite many similar render callings in only once calling.
/// </summary>
public class UnionRender(
    Delegate function,
    params object[] curryingParams
    ) : BaseRender(function, curryingParams)
{
    public override UnionRender Curry(params object?[] args)
        => new(function, [ ..curryingArguments, ..DisplayValues(args) ])
        {
            Context = Context,
            Dependences = Dependences
        };

    protected override IBufferedData FillData(IBufferedData buffer)
    {
        throw new NotImplementedException();
    }

    protected override ShaderObject GenerateDependence(ParameterInfo parameter, int index, object?[] curriedValues)
    {
        throw new NotImplementedException();
    }
}