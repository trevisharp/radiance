/* Author:  Leonardo Trevisan Silio
 * Date:    17/12/2024
 */
using System;
using System.Dynamic;

namespace Radiance.Renders;

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
    /// Validat if a array has any null value.
    /// </summary>
    static object[] ValidateNullValues(object?[] args)
    {
        foreach (var arg in args)
            ArgumentNullException.ThrowIfNull(arg);
        
        return args!;
    }
}