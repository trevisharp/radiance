/* Author:  Leonardo Trevisan Silio
 * Date:    24/12/2024
 */
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Radiance.Bufferings;

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
        
    public static BufferedDataArray operator *(int times, BufferedDataArray stream)
        => new(stream.Select(buffer => times * buffer).ToArray());
    
    public static BufferedDataArray operator *(BufferedDataArray stream, int times)
        => new(stream.Select(buffer => times * buffer).ToArray());
}