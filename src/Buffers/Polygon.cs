/* Author:  Leonardo Trevisan Silio
 * Date:    14/10/2024
 */
using System;
using System.Collections.ObjectModel;

namespace Radiance.Buffers;

/// <summary>
/// A buffer with a Polygon points.
/// </summary>
public class Polygon(float[] data) : IPolygon
{
    private Vec3Buffer triangulationPair = null!;

    /// <summary>
    /// Get the triangulation of this polygon.
    /// </summary>
    public Vec3Buffer Triangules
    {
        get
        {
            triangulationPair ??= FindTriangules();
            return triangulationPair;
        }
    }

    Vec3Buffer FindTriangules()
    {   
        var triangules = Operations
            .PlanarPolygonTriangulation(data[..]);
        
        return new(triangules, 3);
    }

    public float[] GetBufferData()
        => data[..];
        
    public Buffer? Buffer { get; set; }
    
    public int Count => data.Length / 3;
    public int Size => 3;

    public static implicit operator Polygon(float[] data) => new(data);

    public static RepeatPolygon operator *(Polygon polygon, int times)
        => new (polygon, times);

    public static RepeatPolygon operator *(int times, Polygon polygon)
        => new (polygon, times);
}