/* Author:  Leonardo Trevisan Silio
 * Date:    02/12/2024
 */
namespace Radiance.Buffers;

/// <summary>
/// Repeation of a float stream.
/// </summary>
public class RepeatStream(FloatStream data, int times) : IBufferedData
{
    Buffer? buffer = null;

    public int Rows => data.Rows;

    public int Columns => data.Columns;

    public int Instances => times * data.Instances;

    public bool IsGeometry => false;

    public Buffer Buffer => buffer ??= Buffer.From(this);

    public int InstanceLength => data.InstanceLength;

    public float[] GetBufferData()
        => data.GetBufferData();
}