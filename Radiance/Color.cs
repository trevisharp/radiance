/* Author:  Leonardo Trevisan Silio
 * Date:    31/12/2024
 */

namespace Radiance;

using Colors;
using Primitives;
using Exceptions;

public static class Color
{
    /// <summary>
    /// Create a HSV color using h (0 - 360), s (0 - 1) and v (0 - 1).
    /// </summary>
    public static HSV CreateHSV(float h, float s, float v)
        => new(h, s, v);

    
}