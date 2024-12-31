/* Author:  Leonardo Trevisan Silio
 * Date:    31/12/2024
 */
namespace Radiance.Colors;

/// <summary>
/// Represetns a HSV (hue, saturation, value) color.
/// </summary>
public readonly struct HSV(float h, float s, float v)
{
    public readonly float H = h;
    public readonly float S = s;
    public readonly float V = v;
}