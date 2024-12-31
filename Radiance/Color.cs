/* Author:  Leonardo Trevisan Silio
 * Date:    31/12/2024
 */
using System;

namespace Radiance;

using Colors;
using Primitives;

public static class Color
{
    /// <summary>
    /// Create a HSV color using h (0 - 360), s (0 - 1) and v (0 - 1).
    /// </summary>
    public static HSV CreateHSV(float h, float s, float v)
        => new(h, s, v);
    
    /// <summary>
    /// Convert a color for to RGB.
    /// </summary>
    public static Vec3 ToRGB(HSV color)
    {
        float C = color.V * color.S;
        float X = C * (1 - MathF.Abs(color.H / 60 % 2 - 1));
        float m = color.V - C;

        Vec3 temp = color.H switch
        {
            >= 0 and < 60 => (C, X, 0),
            >= 60 and < 120 => (X, C, 0),
            >= 120 and < 180 => (0, C, X),
            >= 180 and < 240 => (0, X, C),
            >= 240 and < 300 => (X, 0, C),
            _ => (C, 0, X),
        };

        return temp + (m, m, m);
    }
}