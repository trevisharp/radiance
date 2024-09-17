/* Author:  Leonardo Trevisan Silio
 * Date:    17/09/2024
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
    protected RenderContext? Context;

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
        => new(function, [ ..curryingArguments, ..args ]);

    public override bool TryInvoke(
        InvokeBinder binder,
        object?[]? args,
        out object? result)
    {
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
        
        var ctx = RenderContext.GetContext();
        if (ctx is null)
        {
            Load();
            CallWithRealData(arguments, parameterCount);
            result = null;
            return true;
        }

        // TODO: Inner render implementation
        CallWithShaderObjects();

        result = null;
        return true;
    }

    /// <summary>
    /// Call the function passing real data and running the draw pipeline.
    /// </summary>
    protected void CallWithRealData(object[] arguments, int parameterCount)
    {
        if (Window.Phase != WindowPhase.OnRender)
            throw new OutOfRenderException();
        
        if (arguments[0] is not Polygon poly)
            throw new MissingPolygonException();
        
        var frameCtx = FrameContext.OpenContext();
        frameCtx.PolygonStack.Push(poly);

        var extraArgs = new object[parameterCount];
        DisplayParameters(extraArgs, arguments[1..]);

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
        
        // TODO: REMOVE DISPLAY ARGS
        var objs = parameters
            .Select((p, i) => GenerateDependence(p, i, curryingArguments.Skip(1).ToArray()))
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
            (true, false, false) => new FloatShaderObject(
                name, ShaderOrigin.Global, [ new UniformFloatDependence(name) ]
            ),

            (false, true, _) => new Sampler2DShaderObject(
                name, ShaderOrigin.FragmentShader, [ new TextureDependence(name) ]
            ),

            (true, false, true) => new FloatShaderObject(
                ShaderObject.ToShaderExpression(curriedValues[index]),
                ShaderOrigin.Global, []
            ),

            _ => throw new InvalidRenderException(parameter)
        };
    }

    /// <summary>
    /// Measure the size of parameters args when displayed based on size.
    /// </summary>
    protected static int MeasureArguments(object[] args)
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
    protected static void DisplayParameters(object[] result, object[] args)
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
    protected static int DisplayParameters(object arg, object[] arr, int index)
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