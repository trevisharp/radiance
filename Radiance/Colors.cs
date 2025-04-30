/* Author:  Leonardo Trevisan Silio
 * Date:    02/01/2025
 */
using System;

namespace Radiance;

using ColorSpaces;

public static class Colors
{
    /// <summary>
    /// Create a RGB color using r (0 - 1), g (0 - 1) and b (0 - 1).
    /// </summary>
    public static RGB CreateRGB(float h, float s, float v)
        => new(h, s, v);

    /// <summary>
    /// Create a HSV color using h (0 - 360), s (0 - 1) and v (0 - 1).
    /// </summary>
    public static HSV CreateHSV(float h, float s, float v)
        => new(h, s, v);
    
    /// <summary>
    /// Convert a color for to RGB.
    /// </summary>
    public static RGB ToRGB(this HSV color)
    {
        float C = color.V * color.S;
        float X = C * (1 - MathF.Abs(color.H / 60 % 2 - 1));
        float m = color.V - C;

        var (tr, tg, tb) = color.H switch
        {
            < 60  => (C, X, 0f),
            < 120 => (X, C, 0f),
            < 180 => (0f, C, X),
            < 240 => (0f, X, C),
            < 300 => (X, 0f, C),
            _     => (C, 0, X)
        };

        return new(tr + m, tg + m, tb + m);
    }
}