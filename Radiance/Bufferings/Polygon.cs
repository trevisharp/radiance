/* Author:  Leonardo Trevisan Silio
 * Date:    02/12/2024
 */
namespace Radiance.Bufferings;

using Internal;
using Exceptions;

/// <summary>
/// A buffer with a Polygon points.
/// </summary>
public class Polygon(float[] data) : IPolygon
{
    Buffer? buffer = null;
    BufferData? pointsPair = null;
    BufferData? boundPair = null;
    BufferData? triangulationPair = null;

    public int Rows => data.Length / 3;

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
            .GetBounds(data[..]);
        
        return CreateBuffer(lines);
    }

    BufferData FindTriangules()
    {
        var triangules = Triangulations
            .PlanarPolygonTriangulation(data[..]);
        
        return CreateBuffer(triangules);
    }

    static BufferData CreateBuffer(float[] points)
    {
        var bufferData = new BufferData(3, points.Length / 3, true);
        bufferData.AddRange(points);
        
        return bufferData;
    }

    public static implicit operator Polygon(float[] data) => new(data);

    public static VirtualPolygons operator *(Polygon polygon, int times)
        => new (polygon, times);

    public static VirtualPolygons operator *(int times, Polygon polygon)
        => new (polygon, times);
        
    public override string ToString()
        => $$"""
        Polygon {
            Rows: {{Rows}},
            Columns: {{Columns}},
            Instances: {{Instances}},
            InstanceLength: {{InstanceLength}},
            IsGeometry: {{IsGeometry}}
        }
        """;
}