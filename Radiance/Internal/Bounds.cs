/* Author:  Leonardo Trevisan Silio
 * Date:    25/06/2025
 */
using System.Collections.Generic;

namespace Radiance.Internal;

/// <summary>
/// A class that contains some util and opeartions.
/// </summary>
public static class Bounds
{
    /// <summary>
    /// Find lines of bounds of a dcel.
    /// </summary>
    public static float[] GetBounds(DCEL dcel)
    {
        var lines = new List<float>();

        foreach (var edge in dcel.Edges)
        {
            var v = edge.From;
            var u = edge.To;
            lines.Add(v.X);
            lines.Add(v.Y);
            lines.Add(v.Z);
            lines.Add(u.X);
            lines.Add(u.Y);
            lines.Add(u.Z);
        }

        return [ ..lines ];
    }
}