/* Author:  Leonardo Trevisan Silio
 * Date:    04/11/2024
 */
using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;
using System.Collections.Generic;

namespace Radiance;

using BufferData;
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
public class Render : DynamicObject
{
    object[] arguments;
    readonly Delegate function;
    readonly int expectedArguments;
    readonly FeatureMap<CallMatch> map = [];
    record CallInfo(
        IEnumerable<ShaderDependence[]> ShaderDependences,
        RenderContext Context
    );
    record CallMatch(
        int[] Depth,
        CallInfo Info
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

        var arguments = DisplayArguments(this.arguments, args);
        var canExecute = !HasSkipValues(arguments);
        var inRenderization = Window.Phase is WindowPhase.OnRender;
        
        if (arguments[0] is not IBufferedData and not SkipCurryingParameter)
            throw new MissingPolygonException();
        
        return (inRenderization, canExecute) switch
        {
            (true, false) => throw new InvalidCurryPhaseException(),
            (true, true) => execute(),
            (false, _) => Curry(this, args)
        };

        object? execute()
        {
            var (deps, ctx) = Load(arguments);
            Invoke(arguments, ctx, deps);
            return null;
        }
    }

    /// <summary>
    /// Load the shader code based on received function.
    /// </summary>
    CallInfo Load(object[] args)
    {
        ValidateDepths(args);
        var depths = DiscoverDepths(args);
        
        var match = map.Get(depths);
        if (match is not null)
            return match.Info;
        
        var nonPolyArgs = RemovePolygonParameter(args);
        var info = AnalisysInvoke(function, nonPolyArgs);
        
        map.Add(depths, new(depths, info));
        return info;
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
    /// </summary>
    static Render Curry(Render render, params object?[] args)
    {
        if (Window.Phase == WindowPhase.OnRender)
            throw new InvalidCurryPhaseException();

        return new(render.function) {
            arguments = DisplayArguments(render.arguments, args)
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
    /// Call the function passing real data and running the draw pipeline.
    /// Match the argument with the lasts dependencies.
    /// </summary>
    static void Invoke(object[] arguments, RenderContext ctx, IEnumerable<ShaderDependence[]> depsconfig)
    {
        if (arguments[0] is not IPolygon)
            throw new MissingPolygonException();
        
        var shaderObjParams = RemovePolygonParameter(arguments);
        var allDeps = depsconfig.SelectMany(x => x);

        foreach (var (arg, dep) in shaderObjParams.Zip(allDeps))
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
    static CallInfo AnalisysInvoke(Delegate function, object[] args)
    {
        var objectData = GenerateObjects(function, args);
        var objs = objectData.Select(data => data.obj);
        var deps = objectData.Select(data => data.dep);

        var ctx = RenderContext.OpenContext();
        function.DynamicInvoke([ ..objs ]);
        RenderContext.CloseContext();

        return new CallInfo(deps, ctx);
    }

    /// <summary>
    /// Recive a function and parameters for this function and generates
    /// ShaderObjects to call the function considering the intent of user
    /// based on args types and sizes. The args may be a array of float
    /// values or float buffers.
    /// </summary>
    static (ShaderObject obj, ShaderDependence[] dep)[] GenerateObjects(Delegate function, object[] args)
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
    static (ShaderObject obj, ShaderDependence[] dep) GenerateObject(ParameterInfo parameter, object[] arguments, ref int layoutLocations)
    {
        var type = parameter.ParameterType;
        var name = parameter.Name!;
        if (type.IsAssignableTo(typeof(val)))
        {
            var x = ToFloatShaderObject(name, arguments[0], ref layoutLocations);
            return (x, [ ..x.Dependencies ]);
        }
        
        if (type.IsAssignableTo(typeof(vec2)))
        {
            var x = ToFloatShaderObject($"{name}1", arguments[0], ref layoutLocations);
            var y = ToFloatShaderObject($"{name}2", arguments[1], ref layoutLocations);
            return (
                Utils.var((x, y), name),
                [ ..x.Dependencies, ..y.Dependencies ]
            );
        }
        
        if (type.IsAssignableTo(typeof(vec3)))
        {
            var x = ToFloatShaderObject($"{name}1", arguments[0], ref layoutLocations);
            var y = ToFloatShaderObject($"{name}2", arguments[1], ref layoutLocations);
            var z = ToFloatShaderObject($"{name}3", arguments[2], ref layoutLocations);
            return (
                Utils.var((x, y, z), name),
                [ ..x.Dependencies, ..y.Dependencies, ..z.Dependencies ]
            );
        }
        
        if (type.IsAssignableTo(typeof(vec4)))
        {
            var x = ToFloatShaderObject($"{name}1", arguments[0], ref layoutLocations);
            var y = ToFloatShaderObject($"{name}2", arguments[1], ref layoutLocations);
            var z = ToFloatShaderObject($"{name}3", arguments[2], ref layoutLocations);
            var w = ToFloatShaderObject($"{name}4", arguments[3], ref layoutLocations);
            return (
                Utils.var((x, y, z, w), name),
                [ ..x.Dependencies, ..y.Dependencies, ..z.Dependencies, ..w.Dependencies ]
            );
        }
        
        if (type.IsAssignableTo(typeof(img)))
        {
            var x = ToSample2DShaderObject(name);
            return (x, [ ..x.Dependencies ]);
        }
            
        throw new UnhandleableArgumentsException(type);
    }

    /// <summary>
    /// Generate a Sampler2DShaderObject object based on their use.
    /// </summary>
    static img ToSample2DShaderObject(string name)
    {
        return new img(
            name,
            ShaderOrigin.FragmentShader,
            [ new TextureDependence(name) ]
        );
    }

    /// <summary>
    /// Generate a FloatShaderObject object based on their use.
    /// </summary>
    static val ToFloatShaderObject(string name, object argument, ref int layoutLocations)
    {
        var isBuffer = argument is IBufferedData;

        return isBuffer switch {

            true => new val(
                name, ShaderOrigin.VertexShader, 
                [ new FloatBufferDependence(name, layoutLocations++) ]
            ),

            false => new val(
                name, ShaderOrigin.Global, 
                [ new UniformFloatDependence(name), ]
            ),

        };
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
        var acctualyTypes = args.Select(arg => arg.GetType());
        var acctualySize = GetTypeSize(acctualyTypes);
        
        if (expectedSize != acctualySize)
            throw new SubRenderArgumentCountException(expectedSize, acctualySize);

        func.DynamicInvoke(args);
    }

    /// <summary>
    /// Remove SKipCurryingParameter values.
    /// </summary>
    static object[] RemoveSkip(object?[] values)
        => values.Where(val => val is not SkipCurryingParameter and not null).ToArray()!;
    
    /// <summary>
    /// Discover the depth of a array of inputs.
    /// </summary>
    static int[] DiscoverDepths(object[] inputs) => [
        ..from input in inputs
        select input switch
        {
            IBufferedData buffer => buffer.Instances,
            _ => 1
        }
    ];

    /// <summary>
    /// Validate recived object depths.
    /// </summary>
    static void ValidateDepths(object[] inputs)
    {
        int? dataSize = null;
        foreach (var input in inputs)
        {
            if (input is not IBufferedData buffered)
                continue;
            var instances = buffered.Instances;
            
            if (dataSize.HasValue && instances != dataSize)
                throw new InvalidDataDepthsException(buffered, instances, dataSize.Value);
            
            dataSize = instances;
        }
    }
}