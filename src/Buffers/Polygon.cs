/* Author:  Leonardo Trevisan Silio
 * Date:    02/12/2024
 */
namespace Radiance.Buffers;

using Internal;

/// <summary>
/// A buffer with a Polygon points.
/// </summary>
public class Polygon : IPolygon
{
    readonly float[] data;
    public float[] Data => data;

    public Polygon(float[] data)
    {
        this.data = data;
        Buffer = Buffer.From(this);
    }

    private Vec3Buffer? triangulationPair = null;
    private Vec3Buffer? boundPair = null;
    private Vec3Buffer? pointsPair = null;

    public Vec3Buffer Triangules
        => triangulationPair ??= FindTriangules();

    public Vec3Buffer Lines
        => boundPair ??= FindBounds();

    public Vec3Buffer Points
        => pointsPair ??= FindPoints();

    Vec3Buffer FindTriangules()
    {   
        var triangules = Triangulations
            .PlanarPolygonTriangulation(data[..]);
        
        return new(triangules, 1, true);
    }

    Vec3Buffer FindBounds()
    {
        var lines = Bounds
            .GetBounds(data[..]);
        
        return new(lines, 1, true);
    }

    Vec3Buffer FindPoints()
    {
        var points = data[..];
        return new (points, 1, true);
    }

    public float[] GetBufferData()
        => data[..];
        
    public Buffer Buffer { get; private set; }
    
    public int Count => data.Length / 3;
    public int Size => 3;
    public int Instances => 1;
    public bool IsGeometry => true;

    public static implicit operator Polygon(float[] data) => new(data);

    public static RepeatPolygon operator *(Polygon polygon, int times)
        => new (polygon, times);

    public static RepeatPolygon operator *(int times, Polygon polygon)
        => new (polygon, times);
}