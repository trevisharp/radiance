/* Author:  Leonardo Trevisan Silio
 * Date:    20/06/2025
 */
using System.Collections.Generic;

namespace Radiance.Internal;

/// <summary>
/// Represents a SweepLine algorithm.
/// </summary>
public class SweepLine
{
    public SweepLine(IEnumerable<Vertex> vertexes)
    {
        orderedVertexes = [ ..vertexes ];
        orderedVertexes.Sort((v, u) => (v.Y - u.Y) switch
        {
            >0 => -1,
            <0 => 1,
            0 when v.X > u.X => -1,
            0 when v.X < u.X => 1,
            _ => 0
        });
    }

    readonly List<Vertex> orderedVertexes;

    public int Length => orderedVertexes.Count;
    
    public Vertex this[int index] => orderedVertexes[index];
}