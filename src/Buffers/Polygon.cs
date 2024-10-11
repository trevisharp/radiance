/* Author:  Leonardo Trevisan Silio
 * Date:    11/10/2024
 */
namespace Radiance.Buffers;

/// <summary>
/// A buffer with a Polygon points.
/// </summary>
public class Polygon(float[] data) : IBufferedData
{
    private TrianguleBuffer triangulationPair = null!;

    /// <summary>
    /// Get the triangulation of this polygon.
    /// </summary>
    public TrianguleBuffer Triangules
    {
        get
        {
            triangulationPair ??= FindTriangules();
            return triangulationPair;
        }
    }

    TrianguleBuffer FindTriangules()
    {   
        var triangules = Operations
            .PlanarPolygonTriangulation([ ..Data ]);
        
        return new(triangules, 3);
    }
    
    public float[] Data => data;
    
    public Buffer? Buffer { get; set; }
    
    public int Vertices => Data.Length / 3;

    public static implicit operator Polygon(float[] data) => new(data);

    public static RepeatPolygon operator *(Polygon polygon, int times)
        => new (polygon, times);

    public static RepeatPolygon operator *(int times, Polygon polygon)
        => new (polygon, times);
}