/* Author:  Leonardo Trevisan Silio
 * Date:    02/12/2024
 */
namespace Radiance.Buffers;

using Internal;

/// <summary>
/// A buffer with a Polygon points.
/// </summary>
public class Polygon(float[] data) : IPolygon
{
    Buffer? buffer = null;
    Vec3Buffer? pointsPair = null;
    Vec3Buffer? boundPair = null;
    Vec3Buffer? triangulationPair = null;

    public int Rows => data.Length / 3;

    public int Columns => 3;
    
    public int Instances => 1;

    public int InstanceLength => Rows;

    public bool IsGeometry => true;

    public Buffer Buffer => buffer ??= Buffer.From(this);

    public Vec3Buffer Triangules
        => triangulationPair ??= FindTriangules();
    
    public Vec3Buffer Lines
        => boundPair ??= FindBounds();
    
    public Vec3Buffer Points
        => pointsPair ??= FindPoints();

    public float[] GetBufferData()
        => data[..];

    Vec3Buffer FindPoints()
    {
        var points = data[..];
        return new (points, 1, true);
    }

    Vec3Buffer FindBounds()
    {
        var lines = Bounds
            .GetBounds(data[..]);
        
        return new(lines, 1, true);
    }

    Vec3Buffer FindTriangules()
    {   
        var triangules = Triangulations
            .PlanarPolygonTriangulation(data[..]);
        
        return new(triangules, 1, true);
    }

    public static implicit operator Polygon(float[] data) => new(data);

    public static RepeatPolygon operator *(Polygon polygon, int times)
        => new (polygon, times);

    public static RepeatPolygon operator *(int times, Polygon polygon)
        => new (polygon, times);
}