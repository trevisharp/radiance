/* Author:  Leonardo Trevisan Silio
 * Date:    26/02/2024
 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Radiance.Data;

using Internal;

/// <summary>
/// A base type for all polygons.
/// </summary>
[CollectionBuilder(typeof(PolygonBuilder), "Create")]
public abstract class Polygon : IEnumerable<float>
{
    internal static class PolygonBuilder
    {
        internal static Polygon Create(ReadOnlySpan<float> values)
        {
            var polygon = new MutablePolygon();
            var it = values.GetEnumerator();
            while (it.MoveNext())
            {
                float x = it.Current;
                if (!it.MoveNext())
                    polygon.add(x, 0, 0);
                
                float y = it.Current;
                if (!it.MoveNext())
                    polygon.add(x, y, 0);
                
                float z = it.Current;
                polygon.add(x, y, z);
            }
            return polygon;
        }
    }

    private MutablePolygon triangulationPair = null;

    /// <summary>
    /// Get the triangulation of this polygon.
    /// </summary>
    public Polygon Triangulation
    {
        get
        {
            if (triangulationPair is not null)
                return triangulationPair;
            
            var triangules = VectorsOperations
                .PlanarPolygonTriangulation(Data.ToArray());
            
            MutablePolygon polygon = new MutablePolygon();
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

    internal int Buffer { get; set; }
    internal int VertexObjectArray { get; set; }
    
    /// <summary>
    /// Get the collection of data points in polygon.
    /// </summary>
    public abstract IEnumerable<float> Data { get; }
    
    /// <summary>
    /// Add a point in polygon with z = 0.
    /// </summary>
    public Polygon Add(Vec2 point)
        => Add(point.X, point.Y, 0);
    
    /// <summary>
    /// Add a point in polygon.
    /// </summary>
    public Polygon Add(Vec3 point)
        => Add(point.X, point.Y, point.Z);
    
    /// <summary>
    /// Add a point in polygon with z = 0.
    /// </summary>
    public Polygon Add(float x, float y)
        => Add(x, y, 0);
    
    /// <summary>
    /// Add a point in polygon.
    /// </summary>
    public Polygon Add(float x, float y, float z)
    {
        add(x, y, z);
        if (OnChange is not null)
            OnChange(true, true);
        return this;
    }

    protected abstract void add(float x, float y, float z);
    
    /// <summary>
    /// Create a copy of this polygon.
    /// </summary>
    public abstract Polygon Clone();

    /// <summary>
    /// Get a immutable copy of this polygon.
    /// </summary>
    public virtual Polygon ToImmutable()
        => new ImmutablePolygon(Data);

    /// <summary>
    /// Event called when data is changed.
    /// </summary>
    public Action<bool, bool> OnChange;

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