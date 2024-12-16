/* Author:  Leonardo Trevisan Silio
 * Date:    13/12/2024
 */
using System;
using System.Collections.Generic;

namespace Radiance.Renders;

using Exceptions;

/// <summary>
/// A chain to handle parameters.
/// </summary>
public class ArgumentHandlerChain
{
    readonly LinkedList<ArgumentHandlerChainLink> chain = [];
    
    /// <summary>
    /// Add new chain link.
    /// </summary>
    public ArgumentHandlerChain Add(ArgumentHandlerChainLink link)
    {
        chain.AddLast(link);
        return this;
    }

    /// <summary>
    /// Add new chain link.
    /// </summary>
    public ArgumentHandlerChain Add<T>()
        where T : ArgumentHandlerChainLink, new()
        => Add(new T());

    /// <summary>
    /// Clear the chain link.
    /// </summary>
    public void Clear()
        => chain.Clear();
    
    public object? HandleArguments(ArgumentHandlerArgs args)
    {
        foreach (var link in chain)
        {
            if (link.CanUpdate(args))
                args = link.Update(args);
            
            if (link.CanHandle(args))
                return link.Handle(args);
        }
        
        throw new UnhandleableArgumentsException();
    }

    static readonly Dictionary<string, ArgumentHandlerChain> chainMap = [];
    public static ArgumentHandlerChain Create(string name, Action<ArgumentHandlerChain> builder)
    {
        if (chainMap.TryGetValue(name, out var chain))
            return chain;
        
        var newChain = new ArgumentHandlerChain();
        builder(newChain);
        chainMap.Add(name, newChain);

        return newChain;
    }
}