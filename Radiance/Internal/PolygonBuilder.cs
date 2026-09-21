/* Author:  Leonardo Trevisan Silio
 * Date:    20/09/2026
 */
using System.Collections.Generic;

namespace Radiance.Internal;

using Bufferings;

/// <summary>
/// A generic builder for polygons, with support for holes.
/// </summary>
public class PolygonBuilder
{
    readonly List<float> contour = [];
    readonly List<float[]> extraRings = [];

    /// <summary>
    /// Get the number of points already added to the contour.
    /// </summary>
    public int Count => contour.Count / 3;

    /// <summary>
    /// Add a external sequential point for this polygon.
    /// </summary>
    public PolygonBuilder AddPoint(float x, float y, float z = 0)
    {
        contour.Add(x);
        contour.Add(y);
        contour.Add(z);
        return this;
    }

    /// <summary>
    /// Open a hole inside this polygon.
    /// </summary>
    public HoleBuilder OpenHole()
        => new(this);

    /// <summary>
    /// Add a closed ring from a ready (x, y, z) sequence. A ring nested inside
    /// an odd number of other rings becomes a hole.
    /// </summary>
    public PolygonBuilder AddRing(float[] points)
    {
        if (points.Length >= 9)
            extraRings.Add(points);
        return this;
    }

    /// <summary>
    /// Add a hole from a ready (x, y, z) sequence.
    /// </summary>
    public PolygonBuilder AddHole(float[] points)
        => AddRing(points);

    /// <summary>
    /// Build a polygon from this builder.
    /// </summary>
    public Polygon Build()
        => contour.Count >= 9
            ? new([ [ ..contour ], ..extraRings ])
            : new([ ..extraRings ]);

    /// <summary>
    /// A builder to create a hole insine another polygon.
    /// </summary>
    public class HoleBuilder(PolygonBuilder parent)
    {
        readonly List<float> points = [];

        /// <summary>
        /// Add a external sequential point for this polygon.
        /// </summary>
        public HoleBuilder AddPoint(float x, float y, float z = 0)
        {
            points.Add(x);
            points.Add(y);
            points.Add(z);
            return this;
        }

        /// <summary>
        /// Build the hole inside the polygon.
        /// </summary>
        public PolygonBuilder Build()
            => parent.AddHole([ ..points ]);
    }
}
