/* Author:  Leonardo Trevisan Silio
 * Date:    31/12/2024
 */
namespace Radiance.Colors;

using Exceptions;

/// <summary>
/// Represetns a HSV (hue, saturation, value) color.
/// </summary>
public readonly struct HSV
{
    public HSV(float h, float s, float v)
    {
        if (h < 0 || h > 360)
            throw new ColorException("HSV", "H", 0, 360, h);
            
        if (s < 0 || s > 1)
            throw new ColorException("HSV", "S", 0, 1, s);
            
        if (v < 0 || v > 1)
            throw new ColorException("HSV", "V", 0, 1, v);
        
        H = h;
        S = s;
        V = v;
    }

    public readonly float H;
    public readonly float S;
    public readonly float V;
}