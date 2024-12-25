/* Author:  Leonardo Trevisan Silio
 * Date:    04/12/2024
 */
namespace Radiance.BufferData;

/// <summary>
/// Virtual repetation of a values of a buffer.
/// </summary>
public class VirtualBufferData(IBufferedData data, int repeat) : IBufferedData
{
    readonly IBufferedData baseData = data;
    readonly int baseRepeat = repeat;
    Buffer? buffer = null;

    public int Rows => baseData.Rows;

    public int Columns => baseData.Columns;

    public int Instances => baseRepeat * baseData.Instances;

    public bool IsGeometry => baseData.IsGeometry;

    public Buffer Buffer => buffer ??= Buffer.From(this);

    public int InstanceLength => baseData.InstanceLength;

    public float[] GetBufferData()
        => baseData.GetBufferData();
        

    public static VirtualBufferData operator *(VirtualBufferData stream, int times)
        => new(stream.baseData, times * stream.baseRepeat);
        
    public static VirtualBufferData operator *(int times, VirtualBufferData stream)
        => new(stream.baseData, times * stream.baseRepeat);
    
    public override string ToString()
        => $$"""
        VirtualBufferData {
            Rows: {{Rows}},
            Columns: {{Columns}},
            Instances: {{Instances}},
            InstanceLength: {{InstanceLength}},
            IsGeometry: {{IsGeometry}}
        }
        """;
}