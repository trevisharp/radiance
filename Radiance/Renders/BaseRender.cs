/* Author:  Leonardo Trevisan Silio
 * Date:    13/12/2024
 */
using System;
using System.Dynamic;

namespace Radiance.Renders;

using Contexts;

public abstract class BaseRender : DynamicObject
{
    public object[] Arguments { get; protected set; } = [];
    public ArgumentHandlerChain Chain { get; private set; } = new();

    public sealed override bool TryInvoke(
        InvokeBinder binder,
        object?[]? args,
        out object? result)
    {
        var arguments = ValidateNullValues(args ?? []);
        result = Chain.HandleArguments(this.Arguments, arguments);
        return true;
    }

    static object[] ValidateNullValues(object?[] args)
    {
        foreach (var arg in args)
            ArgumentNullException.ThrowIfNull(arg);
        
        return args!;
    }
}