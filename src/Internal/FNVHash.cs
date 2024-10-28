/* Author:  Leonardo Trevisan Silio
 * Date:    28/10/2024
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace Radiance.Internal;

/// <summary>
/// A hash dictionary util to map int[]. Usefull for
/// GPU entities based on id.
/// </summary>
public class FeatureMap<T> : IEnumerable<T>
    where T : class
{
    readonly Dictionary<ulong, T> map = [];

    public T? Get(int[] values)
    {
        var hash = CreateFNVHash(values);
        if (map.TryGetValue(hash, out T? value))
            return value;
        
        return null;
    }

    public void Add(int[] values, T data)
    {
        var hash = CreateFNVHash(values);
        map.Add(hash, data);
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

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var pair in map)
            yield return pair.Value;
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}