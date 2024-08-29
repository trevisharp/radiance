/* Author:  Leonardo Trevisan Silio
 * Date:    21/08/2023
 */
namespace Radiance.Internal;

internal static class ParamNamgeGenerator
{
    private static int count = 0;
    internal static string GetNext()
    {
        count++;
        return $"param{count}";
    }
}
