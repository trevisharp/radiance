/* Author:  Leonardo Trevisan Silio
 * Date:    13/12/2024
 */
using System;
using System.Linq;
using System.Dynamic;
using System.Collections.Generic;

namespace Radiance.Renders;

using Buffers;
using Shaders;
using Shaders.Objects;
using Exceptions;
using Primitives;

/// <summary>
/// Represents a function with power to renderize on screen.
/// The behaivour it is defined based on a responsability chain.
/// </summary>
public abstract class BaseRender(
    object[] arguments,
    ArgumentHandlerChain chain
    ) : DynamicObject
{
    public sealed override bool TryInvoke(
        InvokeBinder binder,
        object?[]? args,
        out object? result)
    {
        var newArgs = ValidateNullValues(args ?? []);
        var chainArgs = new ArgumentHandlerArgs {
            Args = arguments,
            NewArgs = newArgs,
            Render = this
        };
        result = chain.HandleArguments(chainArgs);
        return true;
    }

    /// <summary>
    /// Split objects by size and display a array considering skip operations.
    /// </summary>
    public static object[] DisplayArguments(object[] arguments, object?[] newArgs)
    {
        int expectedArguments = arguments.Length;
        var addedValues = SplitObjectsBySize(newArgs);
        return Display(arguments, addedValues, expectedArguments);
    }

    public static object[] GenerateArrayByTypes(IEnumerable<Type> types)
    {
        int expectedArguments = GetExpectedSize(types);
        var paramArray = new object[expectedArguments];

        for (int i = 0; i < paramArray.Length; i++)
            paramArray[i] = Utils.skip;
        
        return paramArray;
    }

    /// <summary>
    /// Get the expected size of values needed to match
    /// with a list of types.
    /// </summary>
    static int GetExpectedSize(IEnumerable<Type> types)
    {
        ArgumentNullException.ThrowIfNull(types, nameof(types));
        return types.Sum(DiscoverSize);
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
                IBufferedData[] data => add(data),
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
    /// Get the size of values needed to use a type.
    /// </summary>
    static int DiscoverSize(Type type)
    {
        if (type == typeof(val))
            return 1;
        
        if (type == typeof(vec2))
            return 2;
        
        if (type == typeof(vec3))
            return 3;
        
        if (type == typeof(vec4))
            return 4;
        
        throw new Exception($"Invalid type '{type}'.");
    }

    /// <summary>
    /// Validat if a array has any null value.
    /// </summary>
    static object[] ValidateNullValues(object?[] args)
    {
        foreach (var arg in args)
            ArgumentNullException.ThrowIfNull(arg);
        
        return args!;
    }
}