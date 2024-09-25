/* Author:  Leonardo Trevisan Silio
 * Date:    24/09/2024
 */
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Radiance.Primitives;

/// <summary>
/// A base type for all polygons and lines.
/// </summary>
public abstract class Polygon : IEnumerable<float>
{
    private Polygon triangulationPair = null!;

    /// <summary>
    /// Get the triangulation of this polygon.
    /// </summary>
    public Polygon Triangulation
    {
        get
        {
            if (triangulationPair is not null)
                return triangulationPair;
            
            var triangules = Operations
                .PlanarPolygonTriangulation(Data.ToArray());
            
            MutablePolygon polygon = [];
            for (int i = 0; i < triangules.Length; i += 3)
                polygon.Add(
                    triangules[i + 0],
                    triangules[i + 1],
                    triangules[i + 2]
                );
            
            polygon.triangulationPair = polygon;
            triangulationPair = polygon;

            return triangulationPair;
        }
    }
    
    /// <summary>
    /// Get the collection of data points in polygon.
    /// </summary>
    public abstract IEnumerable<float> Data { get; }

    /// <summary>
    /// Get the id of the buffer associated with the polygon data.
    /// </summary>
    public int? BufferId { get; set; } = null;
    
    /// <summary>
    /// Add a point in polygon.
    /// </summary>
    public Polygon Add(Vec3 point)
        => Add(point.X, point.Y, point.Z);
    
    /// <summary>
    /// Add a point in polygon.
    /// </summary>
    public Polygon Add(float x, float y, float z)
    {
        AddPoint(x, y, z);
        return this;
    }

    protected abstract void AddPoint(float x, float y, float z);
    
    /// <summary>
    /// Create a copy of this polygon.
    /// </summary>
    public abstract Polygon Clone();

    /// <summary>
    /// Get a immutable copy of this polygon.
    /// </summary>
    public virtual Polygon ToImmutable()
        => new ImmutablePolygon(Data);

    public IEnumerator<float> GetEnumerator()
        => Data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => Data.GetEnumerator();

    public static implicit operator Polygon(List<Vec3> pts)
    {
        var polygon = new MutablePolygon();
        foreach (var pt in pts)
            polygon.Add(pt);
        return polygon;
    }
}