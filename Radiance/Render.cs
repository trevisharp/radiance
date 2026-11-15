/* Author:  Leonardo Trevisan Silio
 * Date:    25/12/2024
 */
using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;
using System.Collections.Generic;

namespace Radiance;

using Bufferings;
using Shaders;
using Shaders.Dependencies;
using Windows;
using Contexts;
using Internal;
using Primitives;
using Exceptions;

/// <summary>
/// A render that unite many similar render callings in only once calling.
/// </summary>
public partial class Render : DynamicObject
{
    object[] arguments;
    readonly Delegate function;
    readonly int expectedArguments;
    readonly FeatureMap<CallInfo> infos = [];
    record CallInfo(
        ArgumentTarget[] Targets,
        RenderContext Context
    );
    readonly record struct ArgumentTarget(
        IFloatArgumentDependence? Float,
        int Component,
        TextureDependence? Texture
    );
    record BufferUse(IBufferedData Buffer, int Column);

    public Render(Delegate renderFunc)
    {
        expectedArguments = GetExpectedArgumentCount(renderFunc) + 1;
        arguments = CreateEmptySkipArray(expectedArguments);
        function = renderFunc;
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
    /// PreLoad this render to avoid performance issues
    /// that may occurs when a render is loaded during
    /// program execution.
    /// </summary>
    public void PreLoad(object[] args)
    {
        throw new NotImplementedException();
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
            MakeSubCall(this, args);
            return null;
        }

        if (Window.Phase is WindowPhase.OnRender)
        {
            GetCall(args).Execute(args);
            return null;
        }

        var arguments = DisplayArguments(this.arguments, args);
        if (arguments[0] is not IBufferedData and not SkipCurryingParameter)
            throw new MissingPolygonException();

        return Curry(this, arguments);
    }

    /// <summary>
    /// Remove the first parameter that need be a polygon.
    /// </summary>
    static object[] RemovePolygonParameter(object[] args)
        => args[1..];

    /// <summary>
    /// Curry parameter of this render fixing it. So f(x, y) and g = f(20) we will have g(10) = f(20, 10).
    /// You can send vec2 or vec3 types to send more than one value at a time, so f(myVec2) is a valid invoke for f.
    /// You can also use skip to currying other paremters, so g = f(Utils.skip, 20) we will have g(10) = f(10, 20).
    /// Do not call this funtion inside Window.OnRender event.
    /// Receives the arguments already laid out, so that a value that costs
    /// something to build, like the polygon of a text, is built only once.
    /// </summary>
    static Render Curry(Render render, object[] arguments)
    {
        if (Window.Phase == WindowPhase.OnRender)
            throw new InvalidCurryPhaseException();

        return new(render.function) {
            arguments = arguments
        };
    }

    /// <summary>
    /// Split objects by size and display a array considering skip operations.
    /// </summary>
    static object[] DisplayArguments(object[] currentArguments, object?[] newArgs)
    {
        var splitedValues = SplitObjectsBySize(newArgs);
        return DisplayValuesOnEmptyPlaces(currentArguments, splitedValues);
    }

    /// <summary>
    /// Get if argument array has a skip value.
    /// </summary>
    static bool HasSkipValues(object[] arguments)
    {
        foreach (var arg in arguments)
        {
            if (arg is SkipCurryingParameter)
                return true;
        }

        return false;
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
                BufferedDataArray array => add([ ..array ]),
                IBufferedData data => add(data),
                Vec2 vec => add(vec.X, vec.Y),
                Vec3 vec => add(vec.X, vec.Y, vec.Z),
                Vec4 vec => add(vec.X, vec.Y, vec.Z, vec.W),
                Texture img => add(img),
                string text => add(CreateText(text)),
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
    /// Turn a text into a polygon and build its triangules right away.
    /// Doing it here keeps the triangulation out of the render phase, where it
    /// would show up as a stutter on the frame the text first appears.
    /// </summary>
    static Polygon CreateText(string text)
        => TextPolygonCache.Get(text);

    /// <summary>
    /// Fill parameters data on a vector skipping values
    /// when using a Utils.Skip or any SkipCurryingParameter object.
    /// This function implements the fact that a render f(x, y)
    /// can curryied by g = f(skip, 20) and so called g(10) where
    /// x = 10 and y = 20.
    /// </summary>
    static object[] DisplayValuesOnEmptyPlaces(object[] arguments, object?[] newArgs)
    {
        var result = new object[arguments.Length];
        for (int i = 0; i < arguments.Length; i++)
            result[i] = arguments[i];
        
        for (int i = 0, j = 0; j < newArgs.Length; i++)
        {
            if (i >= result.Length)
                throw new ExcessOfArgumentsException();

            if (result[i] is not null and not SkipCurryingParameter)
                continue;

            var arg = newArgs[j] ?? 
                throw new CallingNullArgumentException(
                    j == 0 ? null : newArgs[j - 1], j
                );
            j++;

            result[i] = arg;
        }

        return result;
    }

    /// <summary>
    /// Create a array of skip values with a especific size.
    /// </summary>
    static object[] CreateEmptySkipArray(int expectedArguments)
    {
        var arguments = new object[expectedArguments];
        
        for (int i = 0; i < arguments.Length; i++)
            arguments[i] = Utils.skip;
        
        return arguments;
    }

    /// <summary>
    /// Get the count of values that a delegate need to recive
    /// was a render.
    /// </summary>
    static int GetExpectedArgumentCount(Delegate function)
    {
        var parameters = function.GetMethodInfo().GetParameters();
        var types = parameters.Select(p => p.ParameterType);
        return GetTypeSize(types);
    }
    
    /// <summary>
    /// Get the size of values that a set of types can have.
    /// </summary>
    static int GetTypeSize(IEnumerable<Type> types)
        => types.Sum(GetTypeSize);

    /// <summary>
    /// Get the size of values that a type can have.
    /// </summary>
    static int GetTypeSize(Type type)
    {
        if (type.IsAssignableTo(typeof(val)))
            return 1;
        
        if (type.IsAssignableTo(typeof(vec2)))
            return 2;
        
        if (type.IsAssignableTo(typeof(vec3)))
            return 3;
        
        if (type.IsAssignableTo(typeof(vec4)))
            return 4;
        
        if (type.IsAssignableTo(typeof(img)))
            return 1;
        
        throw new UnhandleableArgumentsException(type);
    }

    /// <summary>
    /// Call the function using shader objects to analyze your behaviour.
    /// </summary>
    static CallInfo AnalisysInvoke(Delegate function, object[] args)
    {
        CodeGeneration.AutoVariableName.Reset();
        var objectData = GenerateObjects(function, args);
        var objs = objectData.Select(data => data.obj);
        var targets = objectData.SelectMany(data => data.targets);

        var ctx = RenderContext.OpenContext();
        function.DynamicInvoke([ ..objs ]);
        RenderContext.CloseContext();

        return new CallInfo([ ..targets ], ctx);
    }

    /// <summary>
    /// Recive a function and parameters for this function and generates
    /// ShaderObjects to call the function considering the intent of user
    /// based on args types and sizes. The args may be a array of float
    /// values or float buffers.
    /// </summary>
    static (ShaderObject obj, ArgumentTarget[] targets)[] GenerateObjects(Delegate function, object[] args)
    {
        var parameters = function.Method.GetParameters();
        var result = new ShaderObject[parameters.Length];

        int currentArgumentIndex = 0;
        int layoutLocations = 1;
        return parameters.Select(parameter =>
        {
            var size = GetTypeSize(parameter.ParameterType);
            var endOfParameterArguments = currentArgumentIndex + size;
            var paramArgs = args[currentArgumentIndex..endOfParameterArguments];
            currentArgumentIndex = endOfParameterArguments;
            return GenerateObject(parameter, paramArgs, ref layoutLocations);
        }).ToArray();
    }

    /// <summary>
    /// Generate a object based on their use.
    /// </summary>
    static (ShaderObject obj, ArgumentTarget[] targets) GenerateObject(ParameterInfo parameter, object[] arguments, ref int layoutLocations)
    {
        var type = parameter.ParameterType;
        var name = parameter.Name!;
        if (type.IsAssignableTo(typeof(val)))
        {
            var (x, target) = ToFloatShaderObject(name, arguments[0], ref layoutLocations);
            return (x, [ target ]);
        }

        if (type.IsAssignableTo(typeof(vec2))
            || type.IsAssignableTo(typeof(vec3))
            || type.IsAssignableTo(typeof(vec4)))
            return ToVectorShaderObject(name, arguments, ref layoutLocations);

        if (type.IsAssignableTo(typeof(img)))
        {
            var (x, target) = ToSample2DShaderObject(name);
            return (x, [ target ]);
        }

        throw new UnhandleableArgumentsException(type);
    }

    /// <summary>
    /// Generate a vector object. Values become a single vecN uniform and
    /// buffers become one float attribute per component.
    /// </summary>
    static (ShaderObject obj, ArgumentTarget[] targets) ToVectorShaderObject(string name, object[] arguments, ref int layoutLocations)
    {
        var size = arguments.Length;
        if (!arguments.Any(arg => arg is IBufferedData))
        {
            var dependence = new UniformVectorDependence(name, size);
            ShaderObject uniform = size switch
            {
                2 => new vec2(name, ShaderOrigin.Global, [ dependence ]),
                3 => new vec3(name, ShaderOrigin.Global, [ dependence ]),
                _ => new vec4(name, ShaderOrigin.Global, [ dependence ])
            };
            return (uniform, [ ..Enumerable.Range(0, size).Select(i => new ArgumentTarget(dependence, i, null)) ]);
        }

        var components = new val[size];
        var targets = new ArgumentTarget[size];
        for (int i = 0; i < size; i++)
            (components[i], targets[i]) = ToFloatShaderObject($"{name}{i + 1}", arguments[i], ref layoutLocations);

        ShaderObject vector = size switch
        {
            2 => Utils.var((components[0], components[1]), name),
            3 => Utils.var((components[0], components[1], components[2]), name),
            _ => Utils.var((components[0], components[1], components[2], components[3]), name)
        };
        return (vector, targets);
    }

    /// <summary>
    /// Generate a Sampler2DShaderObject object based on their use.
    /// </summary>
    static (img obj, ArgumentTarget target) ToSample2DShaderObject(string name)
    {
        var dependence = new TextureDependence(name);
        var obj = new img(
            name,
            ShaderOrigin.FragmentShader,
            [ dependence ]
        );
        return (obj, new(null, 0, dependence));
    }

    /// <summary>
    /// Generate a FloatShaderObject object based on their use.
    /// </summary>
    static (val obj, ArgumentTarget target) ToFloatShaderObject(string name, object argument, ref int layoutLocations)
    {
        if (argument is IBufferedData)
        {
            var buffer = new val(
                name, ShaderOrigin.VertexShader,
                [ new FloatBufferDependence(name, layoutLocations++) ]
            );
            return (buffer, default);
        }

        var dependence = new UniformFloatDependence(name);
        var uniform = new val(
            name, ShaderOrigin.Global,
            [ dependence ]
        );
        return (uniform, new(dependence, 0, null));
    }

    /// <summary>
    /// Run this render inside another render.
    /// </summary>
    static void MakeSubCall(Render render, object?[] input)
    {
        var func = render.function;
        var expectedTypes = func.Method
            .GetParameters()
            .Select(p => p.ParameterType);
        var expectedSize = GetTypeSize(expectedTypes);
        
        var args = DisplayValuesOnEmptyPlaces(render.arguments, input);
        args = RemoveSkip(args);
        args = ConvertObjects(args);
        var acctualyTypes = args.Select(arg => arg.GetType());
        var acctualySize = GetTypeSize(acctualyTypes);
        
        if (expectedSize != acctualySize)
            throw new SubRenderArgumentCountException(expectedSize, acctualySize);

        func.DynamicInvoke(args);
    }

    /// <summary>
    /// Convert real values on shader object values
    /// </summary>
    static object[] ConvertObjects(object[] args)
    {
        return args.Select(arg => arg switch
        {
            int value => (val)value,
            float value => (val)value,
            Vec2 value => (vec2)value,
            Vec3 value => (vec3)value,
            Vec4 value => (vec4)value,
            _ => arg
        }).ToArray();
    }

    /// <summary>
    /// Remove SKipCurryingParameter values.
    /// </summary>
    static object[] RemoveSkip(object?[] values)
        => values.Where(val => val is not SkipCurryingParameter and not null).ToArray()!;
}