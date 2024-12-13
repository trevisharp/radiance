/* Author:  Leonardo Trevisan Silio
 * Date:    13/12/2024
 */
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
    public void Add(ArgumentHandlerChainLink link)
        => chain.AddLast(link);

    /// <summary>
    /// Clear the chain link.
    /// </summary>
    public void Clear()
        => chain.Clear();
    
    public object? HandleArguments(object[] arguments, object[] newArguments)
    {
        var args = new ArgumentHandlerArgs(arguments, newArguments);

        foreach (var link in chain)
        {
            if (link.CanUpdate(args))
                args = link.Update(args);
            
            if (link.CanHandle(args))
                return link.Handle(args);
        }
        
        throw new UnhandleableArgumentsException();
    }
}