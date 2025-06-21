/* Author:  Leonardo Trevisan Silio
 * Date:    02/12/2024
 */
namespace Radiance.Bufferings;

using Internal;
using Exceptions;

/// <summary>
/// A buffer with a Polygon points.
/// </summary>
public class DCELPolygon(DCEL dcel) : IPolygon
{
    readonly float[] data = dcel.ToArray();
    Buffer? buffer = null;
    BufferData? pointsPair = null;
    BufferData? boundPair = null;
    BufferData? triangulationPair = null;

    public int Rows => dcel.Length;

    public int Columns => 3;
    
    public int Instances => 1;

    public int InstanceLength => Rows;

    public bool IsGeometry => true;

    public Buffer Buffer => buffer ??= Buffer.From(this);
    
    public Changes Changes { get; set; } = [];

    public IBufferedData Triangules
        => triangulationPair ??= FindTriangules();
    
    public IBufferedData Lines
        => boundPair ??= FindBounds();
    
    public IBufferedData Points
        => pointsPair ??= FindPoints();

    public float this[int index]
    {
        get => data[index];
        set => throw new ImutablePolygonException();
    }

    public float[] GetBufferData()
        => data[..];

    BufferData FindPoints()
    {
        var points = data[..];
        return CreateBuffer(points);
    }

    BufferData FindBounds()
    {
        var lines = Bounds
            .GetBounds(dcel);
        
        return CreateBuffer(lines);
    }

    BufferData FindTriangules()
    {
        var triangules = Triangulations
            .PlanarPolygonTriangulation(dcel);
        
        return CreateBuffer(triangules);
    }

    static BufferData CreateBuffer(float[] points)
    {
        var bufferData = new BufferData(3, points.Length / 3, true);
        bufferData.AddRange(points);
        
        return bufferData;
    }
        
    public override string ToString()
        => $$"""
        DCELPolygon {
            Rows: {{Rows}},
            Columns: {{Columns}},
            Instances: {{Instances}},
            InstanceLength: {{InstanceLength}},
            IsGeometry: {{IsGeometry}}
        }
        """;
}