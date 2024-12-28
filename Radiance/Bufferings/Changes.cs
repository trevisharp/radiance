/* Author:  Leonardo Trevisan Silio
 * Date:    27/12/2024
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace Radiance.Bufferings;

/// <summary>
/// Represents a collection of changes.
/// </summary>
public class Changes : IEnumerable<Change>
{
    const int minDistance = 64;
    readonly List<Change> changes = [];

    /// <summary>
    /// Add a new change.
    /// </summary>
    public void Add(Change newChange)
    {
        Validate(newChange);

        for (int i = 0; i < changes.Count; i++)
        {
            var change = changes[i];
            var distance = Distance(change, newChange);
            if (distance < minDistance)
            {
                changes[i] = Intersect(change, newChange);
                return;
            }
            
            if (newChange.End < change.Start)
            {
                changes.Insert(i, newChange);
                return;
            }
        }

        changes.Add(newChange);
    }

    /// <summary>
    /// Clear all changes.
    /// </summary>
    public void Clear()
        => changes.Clear();

    public IEnumerator<Change> GetEnumerator()
        => changes.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
    
    static void Validate(Change change)
    {
        ArgumentNullException.ThrowIfNull(change, nameof(change));

        if (change.Start < change.End)
            return;
        
        throw new Exception("A change starts after their end.");
    }

    static int Distance(Change change1, Change change2)
    {
        int start = int.Min(change1.Start, change2.Start);
        int end = int.Max(change1.End, change2.End);
        return end - start;
    }

    static Change Intersect(Change change1, Change change2)
    {
        return new Change(
            int.Min(change1.Start, change2.Start),
            int.Max(change1.End, change2.End)
        );
    }
}
