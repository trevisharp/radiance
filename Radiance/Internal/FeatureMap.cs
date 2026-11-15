/* Author:  Leonardo Trevisan Silio
 * Date:    28/10/2024
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace Radiance.Internal;

/// <summary>
/// A hash dictionary util to map int[]. Usefull for
/// GPU entities based on id.
/// </summary>
public class FeatureMap<T> : IEnumerable<T>
    where T : class
{
    readonly Dictionary<ulong, List<(int[] Key, T Value)>> map = [];

    public T? Get(ReadOnlySpan<int> values)
    {
        var hash = CreateFNVHash(values);
        if (!map.TryGetValue(hash, out var entries))
            return null;

        foreach (var (key, value) in entries)
        {
            if (values.SequenceEqual(key))
                return value;
        }

        return null;
    }

    public void Add(ReadOnlySpan<int> values, T data)
    {
        var hash = CreateFNVHash(values);
        if (!map.TryGetValue(hash, out var entries))
            map.Add(hash, entries = []);

        entries.Add((values.ToArray(), data));
    }

    /// <summary>
    /// Generate a hash from a int array.
    /// more info on https://en.wikipedia.org/wiki/Fowler-Noll-Vo_hash_function
    /// </summary>
    static ulong CreateFNVHash(ReadOnlySpan<int> values)
    {
        const ulong fnvPrime = 1099511628211;
        const ulong offsetBasis = 14695981039346656037;

        var hash = offsetBasis;
        foreach (var value in values)
        {
            for (int shift = 0; shift < 32; shift += 8)
                hash = (hash ^ (byte)(value >> shift)) * fnvPrime;
        }

        return hash;
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var entries in map.Values)
        {
            foreach (var (_, value) in entries)
                yield return value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}