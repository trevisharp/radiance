/* Author:  Leonardo Trevisan Silio
 * Date:    06/11/2024
 */
namespace Radiance.Buffers;

/// <summary>
/// Represents a simple triagules buffer with many data on vertices.
/// </summary>
public class Vec3Buffer : IBufferedData
{
    readonly float[] data;
    public Vec3Buffer(float[] data)
    {
        this.data = data;
        Buffer = Buffer.From(this);
    }

    public int Count => data.Length / 3;

    public Buffer Buffer { get; private set; }

    public Vec3Buffer Triangules => this;

    public int Size => 3;

    public float[] GetBufferData()
        => data[..];
}