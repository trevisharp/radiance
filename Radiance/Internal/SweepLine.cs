/* Author:  Leonardo Trevisan Silio
 * Date:    12/06/2025
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace Radiance.Internal;

/// <summary>
/// Represents a SweepLine algorithm.
/// </summary>
public class SweepLine
{
    public SweepLine(Vertex[] vertexes)
    {
        var ordered = vertexes.ToList();
        ordered.Sort((v, u) => (v.Y - u.Y) switch
        {
            >0 => 1,
            <0 => -1,
            0 when v.X > u.X => 1,
            0 when v.X < u.X => -1,
            _ => 0
        });
        orderedVertexes = [ ..ordered ];
    }

    readonly Vertex[] orderedVertexes;

    public int Length => orderedVertexes.Length;
    
    public Vertex this[int index] => orderedVertexes[index];

    public SweepLine ApplyFilter(int[] points)
    {
        throw new NotImplementedException();
    }
}