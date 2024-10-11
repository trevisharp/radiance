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
using System.Buffers;

/// <summary>
/// A render that unite many similar render callings in only once calling.
/// </summary>
public class MultiRender(
    Delegate function,
    params object[] curryingParams
    ) : Render(function, curryingParams)
{
    List<RenderParameterFactory> factories = [ ];
    TrianguleBuffer? data = null;
    bool dataChanged = true;
    
    public override MultiRender Curry(params object?[] args)
        => new(function, [ ..curryingArguments, ..DisplayValues(args) ])
        {
            Context = Context,
            Dependences = Dependences,
            factories = [ 
                ..factories, 
                ..from arg in args 
                    where arg is RenderParameterFactory 
                    select (RenderParameterFactory)arg 
            ]
        };
    
    protected override IBufferedData FillData(IBufferedData buffer)
    {
        if (!dataChanged && data is not null)
            return data;

        dataChanged = false;
        var vertexes = buffer.Triangules.Data;
        data = GetTrianguleBuffer(vertexes);
        return data!;
    }

    TrianguleBuffer GetTrianguleBuffer(float[] vertexes)
    {
        int vertexCount = vertexes.Length / 3;
        int vertexSize = 3 + factories.Count;

        var data = new float[vertexCount * vertexSize];
        var computationResult = new float[factories.Count];

        for (int i = 0; i < vertexCount; i++)
        {
            for (int j = 0; j < computationResult.Length; j++)
                factories[j].GenerateData(i, computationResult, j);
            
            Array.Copy(vertexes, 3 * i, data, vertexSize * i, 3);
            Array.Copy(computationResult, 0, data, vertexSize * i + 3, computationResult.Length);
        }

        return new TrianguleBuffer(data, vertexSize);
    }

    int layoutLocations = 1;
    protected override ShaderObject GenerateDependence(ParameterInfo parameter, int index, object?[] curriedValues)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

        var name = parameter.Name!;
        var isFloat = parameter.ParameterType == typeof(FloatShaderObject);
        var isTexture = parameter.ParameterType == typeof(Sampler2DShaderObject);
        var isConstant = index < curriedValues.Length;
        var isFactory = isConstant && curriedValues[index] is RenderParameterFactory;
        
        return (isFloat, isTexture, isConstant, isFactory) switch
        {
            (true, false, true, true) => new FloatShaderObject(
                name, ShaderOrigin.VertexShader, [ new FloatBufferDependence(name, layoutLocations++) ]
            ),

            (true, false, true, false) => new FloatShaderObject(
                name, ShaderOrigin.FragmentShader, [ new ConstantDependence(name, 
                    curriedValues[index] is float value ? value : throw new Exception($"{curriedValues[index]} is not a float.")) ]
            ),

            (true, false, false, false) => new FloatShaderObject(
                name, ShaderOrigin.Global, [ new UniformFloatDependence(name) ]
            ),

            (false, true, _, false) => new Sampler2DShaderObject(
                name, ShaderOrigin.FragmentShader, [ new TextureDependence(name) ]
            ),

            (false, true, _, true) => throw new NotImplementedException(
                "Radiance not work with texture buffer yet. You cannot use a factory to draw many textures."
            ),

            _ => throw new InvalidRenderException(parameter)
        };
    }
}