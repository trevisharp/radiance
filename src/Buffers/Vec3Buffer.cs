/* Author:  Leonardo Trevisan Silio
 * Date:    16/10/2024
 */
namespace Radiance.Buffers;

/// <summary>
/// Represents a simple triagules buffer with many data on vertices.
/// </summary>
public class Vec3Buffer(float[] buffer) : IBufferedData
{
    public int Count => buffer.Length / 3;

    public Buffer? Buffer { get; set; }

    public Vec3Buffer Triangules => this;

    public int Size => 3;

    public float[] GetBufferData()
        => buffer[..];
}