/* Author:  Leonardo Trevisan Silio
 * Date:    19/09/2024
 */
using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;
using System.Collections.Generic;

namespace Radiance;

using Windows;
using Buffers;
using Shaders;
using Shaders.Objects;
using Shaders.Dependencies;
using Contexts;
using Primitives;
using Exceptions;

/// <summary>
/// Represents a function that can used by GPU to draw in the screen.
/// </summary>
public class Render(
    Delegate function,
    params object?[] curryingArguments
    ) : DynamicObject
{
    /// <summary>
    /// The event called when the render is ready to draw.
    /// </summary>
    protected RenderContext? Context;
    protected ShaderDependence?[]? Dependences;

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
    /// Currying parameters to create a new render.
    /// </summary>
    public Render Curry(params object?[] args)
        => new(function, [ ..curryingArguments, ..DisplayValues(args) ])
        {
            Context = Context,
            Dependences = Dependences
        };

    public override bool TryInvoke(
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

        var parameterCount = function.Method.GetParameters().Length;
        object[] arguments = [
            ..curryingArguments, ..DisplayValues(args ?? [])
        ];

        if (arguments.Length == 0)
        {
            result = this;
            return true;
        }
        
        if (arguments[0] is not Polygon)
            throw new MissingPolygonException();

        if (arguments.Length < parameterCount + 1)
        {
            result = Curry(args ?? []);
            return true;
        }

        if (arguments.Length > parameterCount + 1)
            throw new ExcessOfArgumentsException();
        
        Load();
        CallWithRealData(arguments);
        result = null;
        return true;
    }

    /// <summary>
    /// Call the function passing real data and running the draw pipeline.
    /// </summary>
    protected void CallWithRealData(object[] arguments)
    {
        if (Window.Phase != WindowPhase.OnRender)
            throw new OutOfRenderException();
        
        if (arguments[0] is not Polygon poly)
            throw new MissingPolygonException();
        
        var extraArgs = DisplayValues(arguments[1..]);

        foreach (var (arg, dep) in extraArgs.Zip(Dependences!))
        {
            if (dep is null)
                continue;
            
            dep.UpdateData(arg);
        }

        Context?.Render(poly, extraArgs);
    }
    
    /// <summary>
    /// Call the function using shader objects to analyze behaviour.
    /// </summary>
    protected void CallWithShaderObjects()
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

    /// <summary>
    /// Run this render inside another render.
    /// </summary>
    protected void MakeSubCall(object?[] input)
    {
        var parameters = function.Method.GetParameters();
        var arguments = DisplayShaderObjects([ ..curryingArguments, ..input ]);

        if (parameters.Length != arguments.Length)
            throw new SubRenderArgumentCountException(parameters.Length, arguments.Length);

        function.DynamicInvoke(arguments);
    }

    /// <summary>
    /// Generate a Shader object with dependencies based on ParameterInfo.
    /// </summary>
    protected static ShaderObject GenerateDependence(ParameterInfo parameter, int index, object?[] curriedValues)
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

    /// <summary>
    /// Fill parameters data on a shader object vector.
    /// </summary>
    protected static ShaderObject[] DisplayShaderObjects(object?[] args)
    {
        List<ShaderObject> result = [];

        foreach (var arg in args)
        {
            _ = arg switch
            {
                FloatShaderObject fso => add(fso),
                Sampler2DShaderObject sso => add(sso),

                float value => add(new FloatShaderObject(
                    ShaderObject.ToShaderExpression(value), ShaderOrigin.Global, [ ]
                )),

                double value => add(new FloatShaderObject(
                    ShaderObject.ToShaderExpression(value), ShaderOrigin.Global, [ ]
                )),

                int value => add(new FloatShaderObject(
                    ShaderObject.ToShaderExpression(value), ShaderOrigin.Global, [ ]
                )),

                _ => throw new InvalidPrimitiveException(arg)
            };
        }

        return [.. result];

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
                Polygon poly => add(poly),
                Vec3 vec => add(vec.X, vec.Y, vec.Z),
                Vec2 vec => add(vec.X, vec.Y),
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