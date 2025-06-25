/* Author:  Leonardo Trevisan Silio
 * Date:    29/11/2024
 */
using System.Collections.Generic;

namespace Radiance.Internal;

/// <summary>
/// A class that contains some util and opeartions.
/// </summary>
public static class Bounds
{
    /// <summary>
    /// Find lines of bounds of a polygon.
    /// </summary>
    public static float[] GetBounds(float[] pts)
    {
        var lines = new float[2 * pts.Length];

        lines[^3] = lines[0] = pts[0];
        lines[^2] = lines[1] = pts[1];
        lines[^1] = lines[2] = pts[2];

        for (int i = 3, j = 3; i < pts.Length; i += 3, j += 6)
        {
            lines[j + 0] = pts[i + 0];
            lines[j + 1] = pts[i + 1];
            lines[j + 2] = pts[i + 2];
            
            lines[j + 3] = pts[i + 0];
            lines[j + 4] = pts[i + 1];
            lines[j + 5] = pts[i + 2];
        }

        return lines;
    }

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