/* Author:  Leonardo Trevisan Silio
 * Date:    03/10/2024
 */
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Radiance.Renders;

using Buffers;
using Shaders;
using Shaders.Objects;
using Shaders.Dependencies;
using Primitives;
using Exceptions;
using System.Buffers;

/// <summary>
/// A render that unite many similar render callings in only once calling.
/// </summary>
public class UnionRender(
    Delegate function,
    params object[] curryingParams
    ) : BaseRender(function, curryingParams)
{
    readonly List<object> callings = [ ..curryingParams ];
    readonly SimpleBuffer buffer = new();
    IBufferedData? lastBuffer = null;
    Func<int, bool> breaker = i => i < 1;
    bool dataChanges = true;
    
    /// <summary>
    /// Add a calling pinned argument.
    /// </summary>
    public UnionRender AddArgument(float value)
    {
        dataChanges = true;
        callings.Add(value);
        return this;
    }

    /// <summary>
    /// Add a calling pinned argument.
    /// </summary>
    public UnionRender AddArgument(int value)
    {
        dataChanges = true;
        callings.Add(value);
        return this;
    }
        
    /// <summary>
    /// Add a calling pinned argument.
    /// </summary>
    public UnionRender AddArgument(double value)
    {
        dataChanges = true;
        callings.Add(value);
        return this;
    }
        
    /// <summary>
    /// Add a calling pinned argument.
    /// </summary>
    public UnionRender AddArgument(Texture value)
    {
        dataChanges = true;
        callings.Add(value);
        return this;
    }
    
    /// <summary>
    /// Add a function to compute the value for any calling based on index.
    public UnionRender AddArgumentFactory(Func<int, float> factory)
    {
        dataChanges = true;
        callings.Add(factory);
        return this;
    }
    
    /// <summary>
    /// Set the function that decides when the render need stop.
    /// </summary>
    public UnionRender SetBreaker(Func<int, bool> breaker)
    {
        dataChanges = true;
        this.breaker = breaker;
        return this;
    }

    // TODO: throws error on curry opeartion at a facotried argument.
    public override UnionRender Curry(params object?[] args)
        => new(function, [ ..curryingArguments, ..DisplayValues(args) ])
        {
            Context = Context,
            Dependences = Dependences
        };

    protected override IBufferedData FillData(IBufferedData buffer)
    {
        if (lastBuffer != buffer)
        {
            dataChanges = true;
            lastBuffer = buffer;
        }

        if (!dataChanges)
            return buffer;
        dataChanges = false;

        var vertexes = buffer.Triangulation.Data;
        UpdateData(vertexes);
        return buffer;
    }

    void UpdateData(float[] basicVertexes)
    {
        buffer.Clear();

        Func<int, float>[] computations = callings
            .Where(c => c is Func<int, float> f)
            .Select(c => (Func<int, float>)c)
            .ToArray();
        float[] computationResult = new float[computations.Length];

        int i;
        for (i = 0; i < int.MaxValue; i++)
        {
            if (!breaker(i))
                break;
            
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

        buffer.Vertices = i * basicVertexes.Length;
    }

    protected override ShaderObject GenerateDependence(ParameterInfo parameter, int index, object?[] curriedValues)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

        var layoutLocations = 1;
        var name = parameter.Name!;
        var isFloat = parameter.ParameterType == typeof(FloatShaderObject);
        var isTexture = parameter.ParameterType == typeof(Sampler2DShaderObject);
        var isConstant = index < curriedValues.Length || callings[index] is not Func<int, float>;

        return (isFloat, isTexture, isConstant) switch
        {
            (true, false, false) => new FloatShaderObject(
                name, ShaderOrigin.VertexShader, [ new FloatBufferDependence(name, layoutLocations++) ]
            ),

            (true, false, true) => new FloatShaderObject(
                name, ShaderOrigin.FragmentShader, [ new ConstantDependence(name, 
                    curriedValues[index] is float value ? value : throw new Exception($"{curriedValues[index]} is not a float.")) ]
            ),

            (false, true, true) => new Sampler2DShaderObject(
                name, ShaderOrigin.FragmentShader, [ new TextureDependence(name) ]
            ),

            (false, true, false) => throw new NotImplementedException(
                "Radiance not work with texture buffer yet. Use currying and use only once texture on a multi call"
            ),

            _ => throw new InvalidRenderException(parameter)
        };
    }
}