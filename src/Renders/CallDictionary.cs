/* Author:  Leonardo Trevisan Silio
 * Date:    25/10/2024
 */
using System;
using System.Linq;
using System.Collections.Generic;

namespace Radiance.Renders;

using Shaders;
using Contexts;

/// <summary>
/// A dictionary for register call structure of renders and
/// use the correct RenderContext.
/// </summary>
public class CallDictionary
{
    readonly Dictionary<ulong, CallMatch> map = [];

    public (RenderContext ctx, ShaderDependence[] deps)? GetContext(int[] depth)
    {
        var hash = CreateFNVHash(depth);
        if (map.TryGetValue(hash, out CallMatch? value))
            return (value.Context, value.ShaderDependences);
        
        return null;
    }

    public void AddContext(ShaderDependence[] deps, int[] depth, RenderContext ctx)
    {
        System.Console.WriteLine("add");
        var hash = CreateFNVHash(depth);
        var match = new CallMatch(
            depth, deps, ctx, hash
        );
        map.Add(hash, match);
    } 

    /// <summary>
    /// Generate a hash from a int array.
    /// more info on https://en.wikipedia.org/wiki/Fowler-Noll-Vo_hash_function
    /// </summary>
    static ulong CreateFNVHash(int[] values)
    {
        const ulong fnvPrime = 1099511628211;
        const ulong offsetBasis = 14695981039346656037;
        return values
            .SelectMany(BitConverter.GetBytes)
            .Aggregate(offsetBasis, (hash, b) => (hash ^ b) * fnvPrime);
    }

    record CallMatch(
        int[] Depth,
        ShaderDependence[] ShaderDependences,
        RenderContext Context,
        ulong Hash
    );
}