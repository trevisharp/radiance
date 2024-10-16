/* Author:  Leonardo Trevisan Silio
 * Date:    14/10/2024
 */
namespace Radiance.Buffers;

/// <summary>
/// Represent a object data that can be sended to a GPU Buffer.
/// </summary>
public interface IBufferedData
{
    /// <summary>
    /// Get the count of the vertex.
    /// </summary>
    int Count { get; }
    
    /// <summary>
    /// Get the size of float for each vertex.
    /// </summary>
    int Size { get; }

    /// <summary>
    /// Get the associated buffer.
    /// </summary>
    Buffer? Buffer { get; set; }

    /// <summary>
    /// Generate the data of this buffer.
    /// Prefer to not modify this vector to avoid incorrect behaviours.
    /// </summary>
    float[] GetBufferData();

}