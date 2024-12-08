/* Author:  Leonardo Trevisan Silio
 * Date:    04/12/2024
 */
namespace Radiance.Buffers;

/// <summary>
/// A interface for data types that can be stored in a buffer.
/// </summary>
public interface IBufferizable
{
    /// <summary>
    /// Return the count of flow values.
    /// </summary>
    int ComputeSize();

    /// <summary>
    /// Add values on a buffer starting at a specific index.
    /// </summary>
    void Bufferize(float[] buffer, int index);
}