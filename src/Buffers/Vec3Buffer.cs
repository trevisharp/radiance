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

    public int Rows => data.Length / 3;

    public int Columns => 3;

    public int Instances => instances;

    public int InstanceLength => 1;

    public bool IsGeometry => isGeometry;
    
    public Buffer Buffer => buffer ??= Buffer.From(this);

    public float[] GetBufferData()
        => data[..];
}