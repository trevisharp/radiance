/* Author:  Leonardo Trevisan Silio
 * Date:    09/10/2024
 */
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Radiance.Renders;

using Factories;

using Buffers;
using Shaders;
using Shaders.Objects;
using Shaders.Dependencies;
using Exceptions;

/// <summary>
/// A render that unite many similar render callings in only once calling.
/// </summary>
public class MultiRender(
    Delegate function,
    params object[] curryingParams
    ) : Render(function, curryingParams)
{
    List<object> factories = [ ];
    readonly SimpleBuffer buffer = new();
    IBufferedData? lastBuffer = null;
    Func<int, bool> breaker = i => i < 1;
    bool dataChanges = true;
    
    /// <summary>
    /// Set the function that decides when the render need stop.
    /// </summary>
    public MultiRender SetBreaker(Func<int, bool> breaker)
    {
        dataChanges = true;
        this.breaker = breaker;
        return this;
    }

    public override MultiRender Curry(params object?[] args)
    {
        return new(function, [ ..curryingArguments, ..DisplayValues(args) ])
        {
            Context = Context,
            Dependences = Dependences,
            factories = [ ..factories, ..args.Where(arg => arg is RenderParameterFactory) ],
            breaker = breaker
        };
    }
    protected override IBufferedData FillData(IBufferedData buffer)
    {
        if (lastBuffer != buffer)
        {
            dataChanges = true;
            lastBuffer = buffer;
        }

        if (!dataChanges)
            return this.buffer;
        dataChanges = false;

        var vertexes = buffer.Triangulation.Data;
        UpdateData(vertexes);

        return this.buffer;
    }

    void UpdateData(float[] basicVertexes)
    {
        buffer.Clear();

        Func<int, float>[] computations = factories
            .Where(c => c is Func<int, float> f)
            .Select(c => (Func<int, float>)c)
            .ToArray();
        float[] computationResult = new float[computations.Length];

        int i;
        for (i = 0; breaker(i); i++)
        {
            for (int j = 0; j < computationResult.Length; j++)
                computationResult[j] = computations[j](i);
            
            for (int k = 0; k < basicVertexes.Length; k += 3)
            {
                buffer.Add(basicVertexes[k + 0]);
                buffer.Add(basicVertexes[k + 1]);
                buffer.Add(basicVertexes[k + 2]);
                for (int j = 0; j < computationResult.Length; j++)
                    buffer.Add(computationResult[j]);
            }
        }
        
        buffer.Vertices = i * basicVertexes.Length / 3;
    }

    int layoutLocations = 1;
    protected override ShaderObject GenerateDependence(ParameterInfo parameter, int index, object?[] curriedValues)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

        var name = parameter.Name!;
        var isFloat = parameter.ParameterType == typeof(FloatShaderObject);
        var isTexture = parameter.ParameterType == typeof(Sampler2DShaderObject);
        var hasFactory = index < factories.Count && factories[index] is Func<int, float>;
        var isConstant = index < curriedValues.Length;

        return (isFloat, isTexture, isConstant, hasFactory) switch
        {
            (true, false, false, true) => new FloatShaderObject(
                name, ShaderOrigin.VertexShader, [ new FloatBufferDependence(name, layoutLocations++) ]
            ),

            (true, false, true, false) => new FloatShaderObject(
                name, ShaderOrigin.FragmentShader, [ new ConstantDependence(name, 
                    curriedValues[index] is float value ? value : throw new Exception($"{curriedValues[index]} is not a float.")) ]
            ),

            (true, false, false, false) => new FloatShaderObject(
                name, ShaderOrigin.Global, [ new UniformFloatDependence(name) ]
            ),

            (true, false, true, true) => throw new Exception("A parameter with a factory cannot be curryied."),

            (false, true, _, false) => new Sampler2DShaderObject(
                name, ShaderOrigin.FragmentShader, [ new TextureDependence(name) ]
            ),

            (false, true, _, true) => throw new NotImplementedException(
                "Radiance not work with texture buffer yet. Use currying and use only once texture on a multi call"
            ),

            _ => throw new InvalidRenderException(parameter)
        };
    }

    protected override int CountNeededArguments()
        => base.CountNeededArguments();
}