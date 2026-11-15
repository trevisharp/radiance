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
    readonly IBufferedData[] array = buffers.ToArray();

    /// <summary>
    /// Get the number of buffers on this array.
    /// </summary>
    public int Count => array.Length;

    /// <summary>
    /// Get a buffer of this array.
    /// </summary>
    public IBufferedData this[int index] => array[index];

    public float this[int i, int j]
    {
        get => array[i][j];
        set => array[i][j] = value;
    }

    public IEnumerator<IBufferedData> GetEnumerator()
    {
        foreach (var buffer in array)
            yield return buffer;
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
        
    public static BufferedDataArray operator *(int times, BufferedDataArray stream)
        => new(stream.Select(buffer => times * buffer).ToArray());
    
    public static BufferedDataArray operator *(BufferedDataArray stream, int times)
        => new(stream.Select(buffer => times * buffer).ToArray());
}