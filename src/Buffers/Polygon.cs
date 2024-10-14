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

    readonly ReadOnlyCollection<float> collection = Array.AsReadOnly(data);
    public ReadOnlyCollection<float> Data => collection;
    
    public Buffer? Buffer { get; set; }
    
    public int Count => Data.Count / 3;
    public int Size => 3;

    public static implicit operator Polygon(float[] data) => new(data);

    public static RepeatPolygon operator *(Polygon polygon, int times)
        => new (polygon, times);

    public static RepeatPolygon operator *(int times, Polygon polygon)
        => new (polygon, times);
}