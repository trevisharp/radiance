/* Author:  Leonardo Trevisan Silio
 * Date:    03/12/2024
 */
namespace Radiance.Buffers;

/// <summary>
/// Represents a simple triagules buffer with many data on vertices.
/// </summary>
public class Vec3Buffer(float[] data, int instances, bool isGeometry) : IBufferedData
{
    Buffer? buffer = null;

    public int Count => data.Length / 3;

    public Buffer Buffer => buffer ??= Buffer.From(this);

    public Vec3Buffer Triangules => this;

    public int Size => 3;
    
    public int Instances => instances;

    public bool IsGeometry => isGeometry;

    public float[] GetBufferData()
        => data[..];
}