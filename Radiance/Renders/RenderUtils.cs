/* Author:  Leonardo Trevisan Silio
 * Date:    17/12/2024
 */
using System;
using System.Linq;
using System.Collections.Generic;

namespace Radiance.Renders;

using Buffers;
using Exceptions;
using Primitives;

public static class RenderUtils
{
    /// <summary>
    /// Split objects by size and display a array considering skip operations.
    /// </summary>
    public static object[] DisplayArguments(object[] arguments, object?[] newArgs)
    {
        int expectedArguments = arguments.Length;
        var addedValues = SplitObjectsBySize(newArgs);
        return Display(arguments, addedValues, expectedArguments);
    }

    /// <summary>
    /// Generate a array with skip values based on Type sizes.
    /// </summary>
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
}