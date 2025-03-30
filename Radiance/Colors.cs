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

        RGB temp = color.H switch
        {
            >= 0 and < 60 => new (C, X, 0),
            >= 60 and < 120 => new (X, C, 0),
            >= 120 and < 180 => new (0, C, X),
            >= 180 and < 240 => new (0, X, C),
            >= 240 and < 300 => new (X, 0, C),
            _ => new (C, 0, X),
        };

        return new(temp.R + m, temp.G + m, temp.B + m);
    }
}