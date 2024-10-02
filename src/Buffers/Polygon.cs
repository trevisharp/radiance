/* Author:  Leonardo Trevisan Silio
 * Date:    27/09/2024
 */
namespace Radiance.Buffers;

/// <summary>
/// A base type for all polygons and lines.
/// </summary>
public class Polygon(float[] data) : IBufferedData
{
    private Polygon triangulationPair = null!;

    /// <summary>
    /// Get the triangulation of this polygon.
    /// </summary>
    public IBufferedData Triangulation
    {
        get
        {
            if (triangulationPair is not null)
                return triangulationPair;
            
            var triangules = Operations
                .PlanarPolygonTriangulation([ ..Data ]);
            
            Polygon polygon = new Polygon(triangules);
            
            polygon.triangulationPair = polygon;
            triangulationPair = polygon;

            return triangulationPair;
        }
    }
    
    public float[] Data => data;
    
    public Buffer? Buffer { get; set; }
    
    public int Vertices => Data.Length / 3;

    
    public static implicit operator Polygon(float[] data) => new(data);
}