/* Author:  Leonardo Trevisan Silio
 * Date:    02/01/2024
 */
namespace Radiance.ColorSpaces;

using Exceptions;

/// <summary>
/// Represetns a HSV (hue, saturation, value) color.
/// </summary>
public readonly struct HSV
{
    public HSV(float h, float s, float v)
    {
        if (h is < 0 or > 360)
            throw new ColorException("HSV", "H", 0, 360, h);
            
        if (s is < 0 or > 1)
            throw new ColorException("HSV", "S", 0, 1, s);
            
        if (v is < 0 or > 1)
            throw new ColorException("HSV", "V", 0, 1, v);
        
        H = h;
        S = s;
        V = v;
    }

    public readonly float H;
    public readonly float S;
    public readonly float V;
}