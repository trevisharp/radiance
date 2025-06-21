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
    readonly List<(float x, float y)> planarPoints = [];
    readonly List<List<(float x, float y)>> holes = [];

    /// <summary>
    /// Add a external sequential point for this polygon.
    /// </summary>
    public PolygonBuilder AddPoint(float x, float y)
    {
        planarPoints.Add((x, y));
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
        int id = 0;
        Vertex func((float x, float y) p) => new(id++, p.x, p.y, 0);
        var dcel = new DCEL(
            [ ..planarPoints.Select(func) ],
            [ ..holes.Select(hole => new List<Vertex>(hole.Select(func))) ]
        );

        return new DCELPolygon(dcel);
    }

    /// <summary>
    /// A builder to create a hole insine another polygon.
    /// </summary>
    public class HoleBuilder(PolygonBuilder parent)
    {
        readonly List<(float x, float y)> planarPoints = [];

        /// <summary>
        /// Add a external sequential point for this polygon.
        /// </summary>
        public HoleBuilder AddPoint(float x, float y)
        {
            planarPoints.Add((x, y));
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