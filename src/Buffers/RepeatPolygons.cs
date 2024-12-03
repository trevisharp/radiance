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
    private Buffer? buffer = null;

    public int Count => polygon.Count;
    public int Size => polygon.Size;
    public int Instances => times;
    public bool IsGeometry => true;
    public Buffer Buffer => buffer ??= Buffer.From(this);
    public float[] GetBufferData()
        => polygon.GetBufferData();
    
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
            .PlanarPolygonTriangulation(polygon.Data);
        return new(triangules, times, true);
    }

    Vec3Buffer FindBounds()
    {
        var lines = Bounds
            .GetBounds(polygon.Data);
        return new(lines, times, true);
    }

    Vec3Buffer FindPoints()
    {
        var points = polygon.Data;
        return new(points, times, true);
    }
}