/* Author:  Leonardo Trevisan Silio
 * Date:    07/01/2023
 */
using System.Globalization;

namespace Radiance.Internal;

internal static class InternalExtension
{
    internal static string Format(this float value)
        => value.ToString(CultureInfo.InvariantCulture);
        
    internal static string Format(this double value)
        => value.ToString(CultureInfo.InvariantCulture);
        
    internal static string Format(this int value)
        => value.ToString(CultureInfo.InvariantCulture);
}