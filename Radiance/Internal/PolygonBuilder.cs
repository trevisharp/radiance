/* Author:  Leonardo Trevisan Silio
 * Date:    12/06/2025
 */
using System.Collections.Generic;
using System.Linq;

namespace Radiance.Internal;

using Bufferings;

/// <summary>
/// A generic builder for polygons based DCEL.
/// </summary>
public class PolygonBuilder
{
    readonly List<(float x, float y, float z)> planarPoints = [];
    readonly List<List<(float x, float y, float z)>> holes = [];

    /// <summary>
    /// Add a external sequential point for this polygon.
    /// </summary>
    public PolygonBuilder AddPoint(float x, float y, float z = 0)
    {
        planarPoints.Add((x, y, z));
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
    public IPolygon Build()
    {
        Vertex func((float x, float y, float z) p) => new(p.x, p.y, p.z);
        var dcel = new DCEL(
            [ ..planarPoints.Select(func) ],
            [ ..holes.Select(hole => new List<Vertex>(hole.Select(func))) ]
        );

        return new Polygon(dcel);
    }

    /// <summary>
    /// A builder to create a hole insine another polygon.
    /// </summary>
    public class HoleBuilder(PolygonBuilder parent)
    {
        readonly List<(float x, float y, float z)> planarPoints = [];

        /// <summary>
        /// Add a external sequential point for this polygon.
        /// </summary>
        public HoleBuilder AddPoint(float x, float y, float z = 0)
        {
            planarPoints.Add((x, y, z));
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