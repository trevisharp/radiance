/* Author:  Leonardo Trevisan Silio
 * Date:    03/12/2024
 */
namespace Radiance.Bufferings;

/// <summary>
/// Virtual repetation of a values of a a polygon.
/// </summary>
public class VirtualPolygons(Polygon polygon, int repeat) : IPolygon
{
    Buffer? buffer = null;
    VirtualBufferData? triangulationPair = null;
    VirtualBufferData? boundPair = null;
    VirtualBufferData? pointsPair = null;

    public int Rows => polygon.Rows;

    public int Columns => polygon.Columns;

    public int Instances => repeat;

    public bool IsGeometry => true;

    public Buffer Buffer => buffer ??= Buffer.From(this);

    public int InstanceLength => Rows;

    public IBufferedData Triangules
        => triangulationPair ??= repeat * polygon.Triangules;

    public IBufferedData Lines
        => boundPair ??= repeat * polygon.Lines;

    public IBufferedData Points
        => pointsPair ??= repeat * polygon.Points;
    
    public float[] GetBufferData()
        => polygon.GetBufferData();
    
    public override string ToString()
        => $$"""
        VirtualPolygons {
            Rows: {{Rows}},
            Columns: {{Columns}},
            Instances: {{Instances}},
            InstanceLength: {{InstanceLength}},
            IsGeometry: {{IsGeometry}}
        }
        """;
}