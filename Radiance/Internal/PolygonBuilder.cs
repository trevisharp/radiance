/* Author:  Leonardo Trevisan Silio
 * Date:    25/06/2025
 */
using System.Collections.Generic;

namespace Radiance.Internal;

using Bufferings;

/// <summary>
/// A generic builder for polygons based DCEL.
/// </summary>
public class PolygonBuilder
{
    readonly List<Vertex> planarPoints = [];
    readonly List<List<Vertex>> holes = [];

    /// <summary>
    /// Add a external sequential point for this polygon.
    /// </summary>
    public PolygonBuilder AddPoint(float x, float y, float z = 0)
    {
        planarPoints.Add(new Vertex(x, y, z));
        return this;
    }
    
    /// <summary>
    /// Open a hole inside this polygon.
    /// </summary>
    public HoleBuilder OpenHole()
        => new(this);
    
    /// <summary>
    /// Build a polygon from this builder.
    /// </summary>
    public Polygon Build()
    {
        var dcel = new DCEL(planarPoints, holes);
        return new Polygon(dcel);
    }

    /// <summary>
    /// A builder to create a hole insine another polygon.
    /// </summary>
    public class HoleBuilder(PolygonBuilder parent)
    {
        readonly List<Vertex> planarPoints = [];

        /// <summary>
        /// Add a external sequential point for this polygon.
        /// </summary>
        public HoleBuilder AddPoint(float x, float y, float z = 0)
        {
            planarPoints.Add(new Vertex(x, y, z));
            return this;
        }
        
        /// <summary>
        /// Build the hole inside the polygon.
        /// </summary>
        public PolygonBuilder Build()
        {
            parent.holes.Add(planarPoints);
            return parent;
        }
    }
}