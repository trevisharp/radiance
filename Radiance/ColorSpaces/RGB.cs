/* Author:  Leonardo Trevisan Silio
 * Date:    02/01/2025
 */
namespace Radiance.ColorSpaces;

using Exceptions;
using Primitives;

public readonly struct RGB
{
    public RGB(float r, float g, float b)
    {
        if (r is < 0 or > 1)
            throw new ColorException("RGB", "R", 0, 1, r);
            
        if (g is < 0 or > 1)
            throw new ColorException("RGB", "G", 0, 1, g);
            
        if (b is < 0 or > 1)
            throw new ColorException("RGB", "B", 0, 1, b);
        
        R = r;
        G = g;
        B = b;
    }

    public readonly float R;
    public readonly float G;
    public readonly float B;

    public static implicit operator Vec3(RGB color)
        => new (color.R, color.G, color.B);

    public static implicit operator Vec4(RGB color)
        => new (color.R, color.G, color.B, 1f);
}