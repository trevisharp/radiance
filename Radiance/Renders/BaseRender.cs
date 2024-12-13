/* Author:  Leonardo Trevisan Silio
 * Date:    13/12/2024
 */
using System;
using System.Dynamic;

namespace Radiance.Renders;

using System.Collections.Generic;
using Contexts;

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
        result = chain.HandleArguments(arguments, newArgs);
        return true;
    }

    public static object[] GenerateArrayByTypes(IEnumerable<Type> types)
    {
        int expectedArguments = 0;
        foreach (var type in types)
        {
            
        }

        var paramArray = new object[expectedArguments];
        for (int i = 0; i < paramArray.Length; i++)
            paramArray[i] = Utils.skip;
        return paramArray;
    }

    static object[] ValidateNullValues(object?[] args)
    {
        foreach (var arg in args)
            ArgumentNullException.ThrowIfNull(arg);
        
        return args!;
    }
}