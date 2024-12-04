/* Author:  Leonardo Trevisan Silio
 * Date:    03/12/2024
 */
namespace Radiance.Buffers;

using Internal;

/// <summary>
/// A buffered data with a polygon repeated many times.
/// </summary>
public class RepeatPolygon(Polygon polygon, int times) : IPolygon
{
    Buffer? buffer = null;
    Vec3Buffer? triangulationPair = null;
    Vec3Buffer? boundPair = null;
    Vec3Buffer? pointsPair = null;

    public int Rows => polygon.Rows;

    public int Columns => polygon.Columns;

    public int Instances => times;

    public bool IsGeometry => true;

    public Buffer Buffer => buffer ??= Buffer.From(this);

    public int InstanceLength => Rows;

    public Vec3Buffer Triangules
        => triangulationPair ??= FindTriangules();

    public Vec3Buffer Lines
        => boundPair ??= FindBounds();

    public Vec3Buffer Points
        => pointsPair ??= FindPoints();
    
    public float[] GetBufferData()
        => polygon.GetBufferData();

    // TODO: Change with vec3bufferrepeat
    Vec3Buffer FindTriangules()
    {   
        var triangules = Triangulations
            .PlanarPolygonTriangulation(polygon.GetBufferData());
        return new(triangules, times, true);
    }

    Vec3Buffer FindBounds()
    {
        var lines = Bounds
            .GetBounds(polygon.GetBufferData());
        return new(lines, times, true);
    }

    Vec3Buffer FindPoints()
    {
        var points = polygon.GetBufferData();
        return new(points, times, true);
    }
}