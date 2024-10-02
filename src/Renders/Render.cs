/* Author:  Leonardo Trevisan Silio
 * Date:    02/10/2024
 */
using System;
using System.Reflection;

namespace Radiance.Renders;

using Buffers;
using Shaders;
using Shaders.Objects;
using Shaders.Dependencies;
using Exceptions;

/// <summary>
/// Represents a function that can used by GPU to draw in the screen.
/// </summary>
public class Render(
    Delegate function,
    params object[] curryingArguments
    ) : BaseRender(function, curryingArguments)
{
    /// <summary>
    /// Currying parameters to create a new render.
    /// </summary>
    public override Render Curry(params object?[] args)
        => new(function, [ ..curryingArguments, ..DisplayValues(args) ])
        {
            Context = Context,
            Dependences = Dependences
        };

    protected override IBufferedData FillData(IBufferedData buffer)
        => buffer;

    protected override ShaderObject GenerateDependence(ParameterInfo parameter, int index, object?[] curriedValues)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        
        var name = parameter.Name!;
        var isFloat = parameter.ParameterType == typeof(FloatShaderObject);
        var isTexture = parameter.ParameterType == typeof(Sampler2DShaderObject);
        var isConstant = index < curriedValues.Length;

        return (isFloat, isTexture, isConstant) switch
        {
            (true, false, false) => new FloatShaderObject(
                name, ShaderOrigin.Global, [ new UniformFloatDependence(name) ]
            ),

            (true, false, true) => new FloatShaderObject(
                name, ShaderOrigin.FragmentShader, [ new ConstantDependence(name, 
                    curriedValues[index] is float value ? value : throw new Exception($"{curriedValues[index]} is not a float.")) ]
            ),

            (false, true, _) => new Sampler2DShaderObject(
                name, ShaderOrigin.FragmentShader, [ new TextureDependence(name) ]
            ),

            _ => throw new InvalidRenderException(parameter)
        };
    }
}