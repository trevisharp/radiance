/* Author:  Leonardo Trevisan Silio
 * Date:    18/09/2024
 */
using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;
using System.Collections.Generic;

namespace Radiance.Renders;

using Primitives;
using Exceptions;
using Shaders;
using Shaders.Objects;
using Shaders.Dependencies;
using Windows;

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
        
        var ctx = RenderContext.GetContext();
        if (ctx is null)
        {
            Load();
            CallWithRealData(arguments);
            result = null;
            return true;
        }
        
        throw new NotImplementedException("Inner render call are not implemented yet");
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
        
        var frameCtx = FrameContext.OpenContext();
        frameCtx.PolygonStack.Push(poly);

        var extraArgs = DisplayValues(arguments[1..]);

        foreach (var (arg, dep) in extraArgs.Zip(Dependences!))
        {
            if (dep is null)
                continue;
            
            dep.UpdateData(arg);
        }

        Context?.Render(poly, extraArgs);

        frameCtx.PolygonStack.Pop();
        FrameContext.CloseContext();
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
            (true, false, _) => new FloatShaderObject(
                name, ShaderOrigin.Global, [ new UniformFloatDependence(name) ]
            ),

            (false, true, _) => new Sampler2DShaderObject(
                name, ShaderOrigin.FragmentShader, [ new TextureDependence(name) ]
            ),

            _ => throw new InvalidRenderException(parameter)
        };
    }

    /// <summary>
    /// Fill parameters data on a vector to run a function.
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