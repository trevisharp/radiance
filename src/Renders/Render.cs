/* Author:  Leonardo Trevisan Silio
 * Date:    11/10/2024
 */
using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;
using System.Collections.Generic;

namespace Radiance.Renders;

using Factories;

using Buffers;
using Shaders;
using Shaders.Objects;
using Shaders.Dependencies;
using Windows;
using Contexts;
using Primitives;
using Exceptions;
using System.Collections.ObjectModel;

/// <summary>
/// A render that unite many similar render callings in only once calling.
/// </summary>
public class Render(
    Delegate function,
    params object[] curryingArguments
    ) : DynamicObject
{
    public RenderContext? Context { get; protected set; }
    protected readonly Delegate function = function;
    protected ShaderDependence?[]? Dependences;
    TrianguleBuffer? data = null;
    bool dataChanged = true;
    
    public Render Curry(params object?[] args)
        => new(function, [ ..curryingArguments, ..DisplayValues(args) ])
        {
            Context = Context,
            Dependences = Dependences
        };
    
    protected IPolygon FillData(IPolygon buffer)
    {
        if (!dataChanged && data is not null)
            return data;

        dataChanged = false;
        var vertexes = buffer.Triangules.Data;
        data = GetTrianguleBuffer(vertexes);
        return data!;
    }

    TrianguleBuffer GetTrianguleBuffer(ReadOnlyCollection<float> vertexes)
    {
        throw new NotImplementedException();
        // @@old reference implementation
        // int vertexCount = vertexes.Count / 3;
        // int vertexSize = 3 + factories.Count;

        // var data = new float[vertexCount * vertexSize];
        // var computationResult = new float[factories.Count];

        // for (int i = 0; i < vertexCount; i++)
        // {
        //     for (int j = 0; j < computationResult.Length; j++)
        //         factories[j].GenerateData(i, computationResult, j);
            
        //     for (int k = 0; k < 3; k++)
        //         data[vertexSize * i + k] = vertexes[3 * i + k];
        //     Array.Copy(computationResult, 0, data, vertexSize * i + 3, computationResult.Length);
        // }

        // return new TrianguleBuffer(data, vertexSize);
    }

    int layoutLocations = 1;
    protected ShaderObject GenerateDependence(ParameterInfo parameter, int index, object?[] curriedValues)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

        var name = parameter.Name!;
        var isFloat = parameter.ParameterType == typeof(FloatShaderObject);
        var isTexture = parameter.ParameterType == typeof(Sampler2DShaderObject);
        var isConstant = index < curriedValues.Length;
        var isFactory = isConstant && curriedValues[index] is IMutableData;
        
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
    
    /// <summary>
    /// Get the number of that parameters received for the render to call the function.
    /// </summary>
    protected virtual int CountNeededArguments()
        => function.Method.GetParameters().Length + 1;

    /// <summary>
    /// Call the function passing real data and running the draw pipeline.
    /// Match the argument with the lasts dependencies.
    /// </summary>
    void CallWithRealData(object[] arguments)
    {
        if (Window.Phase != WindowPhase.OnRender)
            throw new OutOfRenderException();
        
        if (arguments[0] is not IPolygon poly)
            throw new MissingPolygonException();
        
        var extraArgs = DisplayValues(arguments[1..]);

        foreach (var (arg, dep) in extraArgs.Zip(Dependences!))
        {
            if (dep is null)
                continue;
            
            dep.UpdateData(arg);
        }
        
        var data = FillData(poly);

        Context?.Render(data);
    }
    
    /// <summary>
    /// Call the function using shader objects to analyze behaviour.
    /// </summary>
    void CallWithShaderObjects()
    {
        var parameters = function.Method.GetParameters();
        
        var objs = parameters
            .Select((p, i) => GenerateDependence(p, i, curryingArguments.Skip(1).ToArray()))
            .ToArray();
        
        Dependences = objs
            .Select(obj => obj.Dependencies.FirstOrDefault())
            .ToArray();

        function.DynamicInvoke(objs);
    }
    
    public sealed override bool TryInvoke(
        InvokeBinder binder, 
        object?[]? args, 
        out object? result)
    {
        if (RenderContext.GetContext() is not null)
        {
            MakeSubCall(args ?? []);
            result = null;
            return true;
        }

        var expectedArguments = CountNeededArguments();
        object[] arguments = [
            ..curryingArguments, ..DisplayValues(args ?? [])
        ];

        if (arguments.Length == 0)
        {
            result = this;
            return true;
        }
        
        if (arguments[0] is not IBufferedData)
            throw new MissingPolygonException();
        
        if (arguments.Length < expectedArguments)
        {
            result = Curry(args ?? []);
            return true;
        }

        if (arguments.Length > expectedArguments)
            throw new ExcessOfArgumentsException();
        
        Load();
        CallWithRealData(arguments);
        result = null;
        return true;
    }
    
    /// <summary>
    /// Create a shader to represent the render.
    /// </summary>
    public void Load()
    {
        if (Context is not null)
            return;

        var ctx = RenderContext.OpenContext();
        CallWithShaderObjects();
        Context = ctx;
        RenderContext.CloseContext();
    }

    /// <summary>
    /// Run this render inside another render.
    /// </summary>
    void MakeSubCall(object?[] input)
    {
        var parameters = function.Method.GetParameters();
        var arguments = DisplayShaderObjects([ ..curryingArguments, ..input ]);

        if (parameters.Length != arguments.Length)
            throw new SubRenderArgumentCountException(parameters.Length, arguments.Length);

        function.DynamicInvoke(arguments);
    }

    /// <summary>
    /// Fill parameters data on a shader object vector.
    /// </summary>
    static ShaderObject[] DisplayShaderObjects(object?[] args)
    {
        List<ShaderObject> result = [];

        foreach (var arg in args)
        {
            _ = arg switch
            {
                FloatShaderObject fso => add(fso),
                Sampler2DShaderObject sso => add(sso),

                Vec2ShaderObject vec => add(vec.x, vec.y),
                Vec3ShaderObject vec => add(vec.x, vec.y, vec.z),
                Vec4ShaderObject vec => add(vec.x, vec.y, vec.z, vec.w),

                Vec2 vec => add(convert(vec.X), convert(vec.Y)),
                Vec3 vec => add(convert(vec.X), convert(vec.Y), convert(vec.Z)),
                Vec4 vec => add(convert(vec.X), convert(vec.Y), convert(vec.Z), convert(vec.W)),

                float value => add(convert(value)),
                double value => add(convert((float)value)),
                int value => add(convert(value)),

                _ => throw new InvalidPrimitiveException(arg)
            };
        }

        return [.. result];

        FloatShaderObject convert(float value)
            => new(ShaderObject.ToShaderExpression(value), ShaderOrigin.Global, []);

        bool add(params ShaderObject[] objs)
        {
            result.AddRange(objs);
            return true;
        }
    }

    /// <summary>
    /// Fill parameters data on a vector.
    /// </summary>
    protected static object[] DisplayValues(object?[] args)
    {
        List<object> result = [];

        foreach (var arg in args)
        {
            _ = arg switch
            {
                IBufferedData poly => add(poly),
                Vec2 vec => add(vec.X, vec.Y),
                Vec3 vec => add(vec.X, vec.Y, vec.Z),
                Vec4 vec => add(vec.X, vec.Y, vec.Z, vec.W),
                Texture img => add(img),
                float num => add(num),
                int num => add((float)num),
                double num => add((float)num),
                float[] sub => add([..sub]),
                RenderParameterFactory fac => add(fac),
                _ => throw new InvalidPrimitiveException(arg)
            };
        }

        return [.. result];

        bool add(params object[] objs)
        {
            result.AddRange(objs);
            return true;
        }
    }
}