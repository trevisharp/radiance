/* Author:  Leonardo Trevisan Silio
 * Date:    20/09/2026
 */
using System.Collections.Generic;

namespace Radiance.Internal;

/// <summary>
/// A class that contains some util and opeartions.
/// </summary>
public static class Bounds
{
    /// <summary>
    /// Find lines of bounds of every closed ring of a polygon.
    /// </summary>
    public static float[] GetBounds(float[][] rings)
    {
        if (rings.Length == 1)
            return GetBounds(rings[0]);

        var lines = new List<float>();

        foreach (var ring in rings)
            lines.AddRange(GetBounds(ring));

        return [ ..lines ];
    }

    /// <summary>
    /// Find lines of bounds of a polygon.
    /// </summary>
    public static float[] GetBounds(float[] pts)
    {
        if (pts.Length < 6)
            return [];

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
}
