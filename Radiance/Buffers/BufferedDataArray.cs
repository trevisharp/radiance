/* Author:  Leonardo Trevisan Silio
 * Date:    24/12/2024
 */
using System.Collections;
using System.Collections.Generic;

namespace Radiance.Buffers;

/// <summary>
/// A collection of buffered data objects.
/// </summary>
public class BufferedDataArray(IEnumerable<IBufferedData> buffers) : IEnumerable<IBufferedData>
{
    public IEnumerator<IBufferedData> GetEnumerator()
    {
        foreach (var buffer in buffers)
            yield return buffer;
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}