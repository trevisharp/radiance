/* Author:  Leonardo Trevisan Silio
 * Date:    24/01/2024
 */
namespace Radiance.Internal;

/// <summary>
/// Get a name for a variable automatically.
/// </summary>
internal static class AutoVariableName
{
    static int variableCount = 0;
    internal static string Next(string type, int count = 4)
    {
        variableCount++;
        var varName =
            type.Length > count ?
            type.Substring(0, count) :
            type;
        return $"{varName}{variableCount}";
    }
}