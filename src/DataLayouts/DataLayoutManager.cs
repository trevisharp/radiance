/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2023
 */
namespace Radiance.DataLayouts;

/// <summary>
/// A global manager for DataLayouts type.
/// </summary>
public static class DataLayoutManager<T>
    where T : DataLayout<T>
{
    private static int bufferObject = int.MinValue;
}