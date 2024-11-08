/* Author:  Leonardo Trevisan Silio
 * Date:    04/11/2024
 */
using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;
using System.Collections.Generic;

namespace Radiance;

using Buffers;
using Shaders;
using Shaders.Objects;
using Shaders.Dependencies;
using Windows;
using Contexts;
using Internal;
using Primitives;
using Exceptions;

/// <summary>
/// A render that unite many similar render callings in only once calling.
/// </summary>
public class Render : DynamicObject
{
    object[] arguments;
    int layoutLocations = 1;
    readonly Delegate function;
    readonly int expectedArguments;

    readonly FeatureMap<CallMatch> map = [];
    record CallMatch(
        int[] Depth,
        ShaderDependence[] ShaderDependences,
        RenderContext Context
    );

    public Render(Delegate function)
    {
        this.function = function;
        expectedArguments = function.Method.GetParameters().Length + 1;

        arguments = new object[expectedArguments];
        for (int i = 0; i < arguments.Length; i++)
            arguments[i] = Utils.skip;
    }
    
    /// <summary>
    /// Curry parameter of this render fixing it. So f(x, y) and g = f(20) we will have g(10) = f(20, 10).
    /// You can send vec2 or vec3 types to send more than one value at a time, so f(myVec2) is a valid invoke for f.
    /// You can also use skip to currying other paremters, so g = f(Utils.skip, 20) we will have g(10) = f(10, 20).
    /// Do not call this funtion inside Window.OnRender event.
    /// </summary>
    public Render Curry(params object?[] args)
    {
        if (Window.Phase == WindowPhase.OnRender)
            throw new InvalidCurryPhaseException();

        return new(function) {
            arguments = DisplayArguments(args)
        };
    }
    
    /// <summary>
    /// Load the shader code based on received function.
    /// </summary>
    public (RenderContext ctx, ShaderDependence[] deps) Load(object[] args)
    {
        var depths = DiscoverDepths(args);
        var info = map.Get(depths);
        if (info is not null)
            return (info.Context, info.ShaderDependences);
        
        var ctx = RenderContext.OpenContext();
        var deps = AnalisysInvoke(args);
        RenderContext.CloseContext();
        
        map.Add(depths, new(depths, deps, ctx));
        return (ctx, deps);
    }

    public sealed override bool TryInvoke(
        InvokeBinder binder, 
        object?[]? args, 
        out object? result)
    {
        result = ReceiveParameters(args ?? []);
        return true;
    }

    /// <summary>
    /// Function called on try execute or curry a render.
    /// This functions decides the render behaviour.
    /// </summary>
    object? ReceiveParameters(object?[] args)
    {
        var inShaderAnalisys = RenderContext.GetContext() is not null;
        if (inShaderAnalisys)
        {
            MakeSubCall(args);
            return null;
        }

        var arguments = DisplayArguments(args);
        var canExecute = arguments.Length == expectedArguments;
        var inRenderization = Window.Phase is WindowPhase.OnRender;

        if (arguments.Length == 0)
            return null;
        
        if (arguments[0] is not IBufferedData and not SkipCurryingParameter)
            throw new MissingPolygonException();

        if (arguments.Length > expectedArguments)
            throw new ExcessOfArgumentsException();
        
        return (inRenderization, canExecute) switch
        {
            (true, false) => throw new InvalidCurryPhaseException(),
            (true, true) => execute(),
            (false, _) => Curry(args)
        };

        object? execute()
        {
            var (ctx, deps) = Load(arguments);
            Invoke(arguments, ctx, deps);
            return null;
        }
    }

    /// <summary>
    /// Call the function passing real data and running the draw pipeline.
    /// Match the argument with the lasts dependencies.
    /// </summary>
    static void Invoke(object[] arguments, RenderContext ctx, ShaderDependence[] deps)
    {
        if (arguments[0] is not IPolygon)
            throw new MissingPolygonException();
        
        var extraArgs = SplitObjectsBySize(arguments[1..]);

        foreach (var (arg, dep) in extraArgs.Zip(deps!))
        {
            if (dep is null)
                continue;
            
            dep.UpdateData(arg);
        }

        ctx?.Render(arguments);
    }
    
    /// <summary>
    /// Call the function using shader objects to analyze your behaviour.
    /// </summary>
    ShaderDependence[] AnalisysInvoke(object[] args)
    {
        var objs = GenerateObjects(args);

        var deps = objs
            .Select(obj => obj.Dependencies.First())
            .ToArray();

        function.DynamicInvoke(objs);

        return deps;
    }

    /// <summary>
    /// Generate objects of this render based on curryied values.
    /// </summary>
    ShaderObject[] GenerateObjects(object[] args)
    {
        var parameters = function.Method.GetParameters();
        
        return parameters.Zip(GenerateDependences(args))
            .Select(x => GenerateObject(x.First.Name!, x.Second))
            .ToArray();
    }
    
    /// <summary>
    /// Discover the depth of a array of inputs.
    /// </summary>
    static int[] DiscoverDepths(object[] inputs)
    {
        return inputs
            .Select(DiscoverDepth)
            .ToArray();
    }

    /// <summary>
    /// Generate a object based on their dependences.
    /// </summary>
    static ShaderObject GenerateObject(string name, ShaderDependence dependence)
    {
        return dependence switch {
            FloatBufferDependence dep => new FloatShaderObject(
                name, ShaderOrigin.VertexShader, [ dep ]),

            UniformFloatDependence dep => new FloatShaderObject(
                name, ShaderOrigin.Global, [ dep ]),
            
            TextureDependence dep => new Sampler2DShaderObject(
                name, ShaderOrigin.FragmentShader, [ dep ]),
            
            _ => throw new NotImplementedException()
        };
    }

    /// <summary>
    /// Generate the dependencies from this calling. 
    /// </summary>
    ShaderDependence[] GenerateDependences(object[] args)
    {
        var parameters = function.Method.GetParameters();
        var nonPolyArgs = args.Skip(1).ToArray();
        var deps = parameters.Select(
            (p, i) => GenerateDependence(p, i, nonPolyArgs)
        );
        return [ ..deps ];
    }

    /// <summary>
    /// Generate a ShaderObject based on a paramter and curryiedValues.
    /// </summary>
    ShaderDependence GenerateDependence(ParameterInfo parameter, int index, object?[] args)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

        var name = parameter.Name!;
        var isFloat = parameter.ParameterType == typeof(FloatShaderObject);
        var isTexture = parameter.ParameterType == typeof(Sampler2DShaderObject);
        var isBuffer = args[index] is IBufferedData;
        
        return (isFloat, isTexture, isBuffer) switch
        {
            (true, false, true) => new FloatBufferDependence(name, layoutLocations++),

            (true, false, false) => new UniformFloatDependence(name),

            (false, true, false) => new TextureDependence(name),

            _ => throw new InvalidRenderException(parameter)
        };
    }

    /// <summary>
    /// Run this render inside another render.
    /// </summary>
    void MakeSubCall(object?[] input)
    {
        var parameters = function.Method.GetParameters();
        var args = SplitShaderObjectsBySide(input);
        args = Display(arguments, args, expectedArguments);
        args = RemoveSkip(args);
        
        if (parameters.Length != args.Length)
            throw new SubRenderArgumentCountException(parameters.Length, args.Length - 1);

        function.DynamicInvoke(args);
    }

    /// <summary>
    /// Remove SKipCurryingParameter values.
    /// </summary>
    static object[] RemoveSkip(object[] values)
        => values.Where(val => val is not SkipCurryingParameter).ToArray();

    /// <summary>
    /// Split objects by size and display a array considering skip operations.
    /// </summary>
    object[] DisplayArguments(object?[] newArgs)
    {
        var addedValues = SplitObjectsBySize(newArgs);
        return Display(arguments, addedValues, expectedArguments);
    }

    /// <summary>
    /// Fill parameters data on a vector skipping values
    /// when using a Utils.Skip or any SkipCurryingParameter object.
    /// This function implements the fact that a render f(x, y)
    /// can curryied by g = f(skip, 20) and so called g(10) where
    /// x = 10 and y = 20.
    /// </summary>
    static object[] Display(object[] values, object?[] addedValues, int newValueSize)
    {
        var newValues = new object[newValueSize];
        for (int i = 0; i < values.Length; i++)
            newValues[i] = values[i];
        
        for (int i = 0, j = 0; i < addedValues.Length; j++)
        {
            if (newValues[j] is not null and not SkipCurryingParameter)
                continue;

            var arg = addedValues[i] ?? 
                throw new CallingNullArgumentException(
                    i == 0 ? null : addedValues[i - 1], i
                );
            i++;

            newValues[j] = arg;
        }

        return newValues;
    }

    /// <summary>
    /// Fill parameters data on a shader object vector based on their sizes.
    /// This function implements the fact that a render f(x, y)
    /// can be called by f(v) wheres v is a vec2 with 2 values.
    /// </summary>
    static object[] SplitShaderObjectsBySide(object?[] args)
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
    /// Fill parameters data on a vector based on their sizes.
    /// This function implements the fact that a render f(x, y)
    /// can be called by f(v) wheres v is a vec2 with 2 values.
    /// </summary>
    static object[] SplitObjectsBySize(object?[] args)
    {
        List<object> result = [];

        foreach (var arg in args)
        {
            _ = arg switch
            {
                IBufferedData data => add(data),
                Vec2 vec => add(vec.X, vec.Y),
                Vec3 vec => add(vec.X, vec.Y, vec.Z),
                Vec4 vec => add(vec.X, vec.Y, vec.Z, vec.W),
                Texture img => add(img),
                float num => add(num),
                int num => add((float)num),
                double num => add((float)num),
                float[] sub => add([..sub]),
                SkipCurryingParameter skip => add(skip),
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

    /// <summary>
    /// Get the object depth.
    /// </summary>
    static int DiscoverDepth(object obj)
        => obj switch
        {
            IBufferedData buffer => buffer.Count,
            _ => 1
        };
}