/* Author:  Leonardo Trevisan Silio
 * Date:    12/09/2024
 */
using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;

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
    protected Action<Polygon, object[]>? OnRender;

    /// <summary>
    /// Create a shader to represent the render.
    /// </summary>
    public void Load()
    {
        var ctx = RenderContext.OpenContext();

        CallWithShaderObjects();

        OnRender += ctx.RenderActions;

        RenderContext.CloseContext();
    }

    /// <summary>
    /// Currying parameters to create a new render.
    /// </summary>
    public Render Curry(params object?[] args)
        => new(function, [ ..curryingArguments, ..args ]);

    public override bool TryInvoke(
        InvokeBinder binder,
        object?[]? args,
        out object? result)
    {
        var ctx = RenderContext.GetContext();
        var parameterCount = function.Method.GetParameters().Length;
        object[] arguments = [
            ..curryingArguments, ..args
        ];
        var argumentCount = MeasureArguments(arguments);

        if (argumentCount == 0)
        {
            result = this;
            return true;
        }
        
        if (arguments[0] is not Polygon)
            throw new MissingPolygonException();

        if (argumentCount < parameterCount + 1)
        {
            result = Curry(args ?? []);
            return true;
        }

        if (argumentCount > parameterCount + 1)
            throw new ExcessOfArgumentsException();
        
        if (ctx is null)
        {
            CallWithRealData(arguments, parameterCount);
            result = null;
            return true;
        }

        CallWithShaderObjects();

        result = null;
        return true;
    }

    /// <summary>
    /// Call the function passing real data and running the draw pipeline.
    /// </summary>
    void CallWithRealData(object[] arguments, int parameterCount)
    {
        if (Window.Phase == WindowPhase.None)
            throw new OutOfRenderException();
        
        if (arguments[0] is not Polygon poly)
            throw new MissingPolygonException();
        
        var frameCtx = FrameContext.OpenContext();
        frameCtx.PolygonStack.Push(poly);

        var extraArgs = new object[parameterCount];
        DisplayParameters(extraArgs, arguments[1..]);

        if (OnRender is not null)
            OnRender(poly, extraArgs);

        frameCtx.PolygonStack.Pop();
        FrameContext.CloseContext();
        
    }
    
    /// <summary>
    /// Call the function using shader objects to analyze behaviour.
    /// </summary>
    void CallWithShaderObjects()
    {
        var parameters = function.Method.GetParameters();
        
        var objs = parameters
            .Select(GenerateDependence)
            .ToArray();
        
        if (objs.Any(obj => obj is null))
            throw new InvalidRenderException();

        function.DynamicInvoke(objs);
    }

    /// <summary>
    /// Generate a Shader object with dependencies based on ParameterInfo.
    /// </summary>
    static ShaderObject? GenerateDependence(ParameterInfo parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        var name = parameter.Name!;

        if (parameter.ParameterType == typeof(FloatShaderObject))
        {
            var dep = new UniformFloatDependence(name);
            return new FloatShaderObject(
                name, ShaderOrigin.Global, [dep]
            );
        }

        if (parameter.ParameterType == typeof(Sampler2DShaderObject))
        {
            var dep = new TextureDependence(name);
            return new Sampler2DShaderObject(
                name, ShaderOrigin.FragmentShader, [dep]
            );
        }
        
        return null;
    }

    /// <summary>
    /// Measure the size of parameters args when displayed based on size.
    /// </summary>
    static int MeasureArguments(object[] args)
        => args.Sum(arg => arg switch
        {
            Vec2 or Vec2ShaderObject => 2,
            Vec3 or Vec3ShaderObject => 3,
            Vec4 or Vec4ShaderObject => 4,
            float[] arr => arr.Length,
            _ => 1
        });

    /// <summary>
    /// Fill parameters data on a vector to run a function.
    /// </summary>
    static void DisplayParameters(object[] result, object[] args)
    {
        int index = 0;
        foreach (var arg in args)
            index = DisplayParameters(arg, result, index);
        
        if (index < args.Length)
            throw new ExcessOfArgumentsException();
    }
    
    /// <summary>
    /// Set arr data from a index based on arg data size.
    /// </summary>
    static int DisplayParameters(object arg, object[] arr, int index)
    {
        return arg switch
        {
            Vec4 vec => verifyAndAdd(vec.X, vec.Y, vec.Z, vec.W),
            Vec3 vec => verifyAndAdd(vec.X, vec.Y, vec.Z),
            Vec2 vec => verifyAndAdd(vec.X, vec.Y),
            Texture img => verifyAndAdd(img),
            float num => verifyAndAdd(num),
            int num => verifyAndAdd((float)num),
            double num => verifyAndAdd((float)num),
            float[] sub => verifyAndAdd([..sub]),
            _ => throw new InvalidPrimitiveException(arg)
        };

        int verifyAndAdd(params object[] objs)
        {
            verify(objs.Length);
            foreach (var obj in objs)
                add(obj);
            
            return index;
        }

        void verify(int size)
        {
            if (index + size <= arr.Length)
                return;
            
            throw new ExcessOfArgumentsException();
        }

        void add(object value)
            => arr[index++] = value;
    }
}