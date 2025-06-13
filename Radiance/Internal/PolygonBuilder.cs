/* Author:  Leonardo Trevisan Silio
 * Date:    12/06/2025
 */
using System;
using System.Collections.Generic;

namespace Radiance.Internal;

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
    public DCEL Build()
    {
        throw new NotImplementedException();
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