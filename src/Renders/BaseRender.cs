/* Author:  Leonardo Trevisan Silio
 * Date:    02/10/2024
 */
using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;
using System.Collections.Generic;

namespace Radiance.Renders;

using Buffers;
using Shaders;
using Windows;
using Shaders.Objects;
using Contexts;
using Primitives;
using Exceptions;

public abstract class BaseRender(
    Delegate function, params object[] curryingArguments
    ) : DynamicObject
{
    public RenderContext? Context { get; protected set; }
    protected readonly Delegate function = function;
    protected  readonly object[] curryingArguments = curryingArguments;
    protected ShaderDependence?[]? Dependences;

    /// <summary>
    /// Currying parameters to create a new render.
    /// </summary>
    public abstract BaseRender Curry(params object?[] args);

    /// <summary>
    /// Generate a Shader object with dependencies based on ParameterInfo. Recive the parameterInfo,
    /// the index of the parameter and the array of curryied values without the curryied polygon.
    /// </summary>
    protected abstract ShaderObject GenerateDependence(ParameterInfo parameter, int index, object?[] curryiedValues);

    /// <summary>
    /// Fill or transform the data to perform the correct rendering operations.
    /// </summary>
    protected abstract IBufferedData FillData(IBufferedData buffer);

    protected virtual int CountNeededArguments()
        => function.Method.GetParameters().Length + 1;

    /// <summary>
    /// Call the function passing real data and running the draw pipeline.
    /// </summary>
    void CallWithRealData(object[] arguments)
    {
        if (Window.Phase != WindowPhase.OnRender)
            throw new OutOfRenderException();
        
        if (arguments[0] is not IBufferedData poly)
            throw new MissingPolygonException();
        
        var extraArgs = DisplayValues(arguments[1..]);

        foreach (var (arg, dep) in extraArgs.Zip(Dependences!))
        {
            if (dep is null)
                continue;
            
            dep.UpdateData(arg);
        }

        poly = FillData(poly);

        Context?.Render(poly, extraArgs);
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