/* Author:  Leonardo Trevisan Silio
 * Date:    13/12/2024
 */
using System;
using System.Dynamic;

namespace Radiance.Renders;

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

    static object[] ValidateNullValues(object?[] args)
    {
        foreach (var arg in args)
            ArgumentNullException.ThrowIfNull(arg);
        
        return args!;
    }
}