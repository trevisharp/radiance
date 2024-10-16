/* Author:  Leonardo Trevisan Silio
 * Date:    16/10/2024
 */
namespace Radiance.Buffers;

/// <summary>
/// Represents a simple triagules buffer with many data on vertices.
/// </summary>
public class Vec3Buffer(float[] buffer, int vertexDataSize) : IBufferedData
{
    public int Count => buffer.Length / vertexDataSize;

    public Buffer? Buffer { get; set; }

    public Vec3Buffer Triangules => this;

    public int Size => vertexDataSize;

    public float[] GetBufferData()
        => buffer[..];
}