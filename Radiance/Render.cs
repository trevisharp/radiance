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
    int layoutLocations = 1;
    object[] arguments;
    readonly Delegate function;
    readonly int expectedArguments;
    readonly FeatureMap<CallMatch> map = [];  // To Validate
    record CallMatch(  // To Validate
        int[] Depth,
        ShaderDependence[] ShaderDependences,
        RenderContext Context
    );
    record BufferUse(IBufferedData Buffer, int Column);

    public Render(Delegate renderFunc)
    {
        expectedArguments = GetExpectedArgumentCount(renderFunc);
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
    /// Load the shader code based on received function.
    /// </summary>
    public (RenderContext ctx, ShaderDependence[] deps) Load(object[] args) // To Validate
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
            var (ctx, deps) = Load(arguments);
            Invoke(arguments, ctx, deps);
            return null;
        }
    }

    /// <summary>
    /// Run this render inside another render.
    /// </summary>
    static void MakeSubCall(Render render, object?[] input) // To Validate
    {
        var func = render.function;
        var parameters = func.Method.GetParameters();
        var args = SplitShaderObjectsBySide(input);
        args = DisplayOnSkipPlaces(render.arguments, args);
        args = RemoveSkip(args);
        
        if (parameters.Length != args.Length)
            throw new SubRenderArgumentCountException(parameters.Length, args.Length);

        func.DynamicInvoke(args);
    }
    
    /// <summary>
    /// Curry parameter of this render fixing it. So f(x, y) and g = f(20) we will have g(10) = f(20, 10).
    /// You can send vec2 or vec3 types to send more than one value at a time, so f(myVec2) is a valid invoke for f.
    /// You can also use skip to currying other paremters, so g = f(Utils.skip, 20) we will have g(10) = f(10, 20).
    /// Do not call this funtion inside Window.OnRender event.
    /// </summary>
    static Render Curry(Render render, params object?[] args) // To Validate
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
    static object[] DisplayArguments(object[] currentArguments, object?[] newArgs) // To Validate
    {
        var splitedValues = SplitObjectsBySize(newArgs);
        return DisplayOnSkipPlaces(currentArguments, splitedValues);
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
                IPolygon polygon => add(polygon),
                IBufferedData data => addBuffer(data),
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

        bool addBuffer(IBufferedData data)
        {
            for (int i = 0; i < data.Columns; i++)
                result.Add(new BufferUse(data, i));
            return true;
        }

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
    static object[] DisplayOnSkipPlaces(object[] arguments, object?[] newArgs)
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
        return GetTypeSize(types) + 1;
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
        if (type.IsAssignableTo(typeof(FloatShaderObject)))
            return 1;
        
        if (type.IsAssignableTo(typeof(Vec2ShaderObject)))
            return 2;
        
        if (type.IsAssignableTo(typeof(Vec3ShaderObject)))
            return 3;
        
        if (type.IsAssignableTo(typeof(Vec4ShaderObject)))
            return 4;
        
        if (type.IsAssignableTo(typeof(Sampler2DShaderObject)))
            return 1;
        
        throw new UnhandleableArgumentsException(type);
    }

    /// <summary>
    /// Call the function passing real data and running the draw pipeline.
    /// Match the argument with the lasts dependencies.
    /// </summary>
    static void Invoke(object[] arguments, RenderContext ctx, ShaderDependence[] deps) // To Validate
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
    ShaderDependence[] AnalisysInvoke(object[] args) // To Validate
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
    ShaderObject[] GenerateObjects(object[] args) // To Validate
    {
        var parameters = function.Method.GetParameters();
        
        return parameters.Zip(GenerateDependences(args))
            .Select(x => GenerateObject(x.First.Name!, x.Second))
            .ToArray();
    }

    /// <summary>
    /// Generate a object based on their dependences.
    /// </summary>
    static ShaderObject GenerateObject(string name, ShaderDependence dependence) // To Validate
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
    ShaderDependence[] GenerateDependences(object[] args) // To Validate
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
    ShaderDependence GenerateDependence(ParameterInfo parameter, int index, object?[] args) // To Validate
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
    /// Remove SKipCurryingParameter values.
    /// </summary>
    static object[] RemoveSkip(object[] values) // To Validate
        => values.Where(val => val is not SkipCurryingParameter).ToArray();

    /// <summary>
    /// Fill parameters data on a shader object vector based on their sizes.
    /// This function implements the fact that a render f(x, y)
    /// can be called by f(v) wheres v is a vec2 with 2 values.
    /// </summary>
    static object[] SplitShaderObjectsBySide(object?[] args) // To Validate
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
    /// Discover the depth of a array of inputs.
    /// </summary>
    static int[] DiscoverDepths(object[] inputs) => [ // To Validate
        ..from input in inputs
        select input switch
        {
            IBufferedData buffer => buffer.Rows,
            _ => 1
        }
    ];
}