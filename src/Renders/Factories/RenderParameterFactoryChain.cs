/* Author:  Leonardo Trevisan Silio
 * Date:    09/10/2024
 */
using System.Collections;
using System.Collections.Generic;

namespace Radiance.Renders.Factories;

using Exceptions;

/// <summary>
/// A chain of handlers to create factories
/// </summary>
public class RenderParameterFactoryChain : IEnumerable<RenderParameterFactoryChainLink>
{
    public RenderParameterFactoryChainLink? First { get; private set; }
    public RenderParameterFactoryChainLink? Last { get; private set; }

    public void Add(RenderParameterFactoryChainLink link)
    {
        if (First is null || Last is null)
        {
            First = link;
            Last = link;
            return;
        }

        Last.Next = link;
        Last = link;
    }

    public void Clear()
        => First = Last = null;

    public RenderParameterFactory ToFactory(object value)
    {
        var it = First;
        while (it is not null)
        {
            if (it.Is(value))
                return it.Create(value);
            
            it = it.Next;
        }

        throw new InvalidFactoryDataException();
    }

    public IEnumerator<RenderParameterFactoryChainLink> GetEnumerator()
    {
        var it = First;
        while (it is not null)
        {
            yield return it;
            it = it.Next;
        }
    }
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}