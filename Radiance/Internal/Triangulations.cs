/* Author:  Leonardo Trevisan Silio
 * Date:    20/09/2026
 */
using System;
using System.Collections.Generic;

namespace Radiance.Internal;

/// <summary>
/// A class that contains some util and opeartions.
/// </summary>
public static class Triangulations
{
    /// <summary>
    /// Get a triangulation of a simple polygon received as a (x, y, z) sequence.
    /// The vertices may be given clockwise or counterclockwise and the polygon
    /// may be convex or concave. The triangules are always emitted
    /// counterclockwise on the xy plane, keeping the z of each original vertex.
    /// </summary>
    public static float[] PlanarPolygonTriangulation(float[] pts)
    {
        var vertexCount = pts.Length / 3;
        if (vertexCount < 3)
            return pts;

        var indices = DropRepeatedVertices(pts, vertexCount);
        if (indices.Count < 3)
            return pts;

        var epsilon = AreaEpsilon(pts, indices);
        DropCollinearVertices(pts, indices, epsilon);

        if (SignedArea(pts, indices) < 0)
            indices.Reverse();

        var triangules = new List<float>(9 * (indices.Count - 2));
        EarClipping(pts, indices, triangules, epsilon);

        return triangules.Count > 0 ? [ ..triangules ] : pts;
    }

    /// <summary>
    /// Clip one ear at a time until only a triangule remains.
    /// Every iteration drops exactly one vertex, so the loop always ends.
    /// </summary>
    static void EarClipping(float[] pts, List<int> indices, List<float> triangules, double epsilon)
    {
        while (indices.Count > 3)
        {
            // A vertex is an ear when it is convex and no other vertex of the
            // polygon falls inside the triangule it forms with its neighbours.
            var ear = FindEar(pts, indices, epsilon, allowBoundaryContact: false);

            // Vertices touching the border of the candidate triangule are
            // tolerated on a second pass: they do not overlap any area.
            if (ear < 0)
                ear = FindEar(pts, indices, epsilon, allowBoundaryContact: true);

            // Only reached by self intersecting or fully degenerated input,
            // where no real ear exists. Clipping the sharpest vertex keeps
            // the output finite instead of looping forever.
            if (ear < 0)
                ear = SharpestVertex(pts, indices);

            AddTriangule(pts, triangules, indices, ear, epsilon);
            indices.RemoveAt(ear);
        }

        AddTriangule(pts, triangules, indices, 1, epsilon);
    }

    /// <summary>
    /// Find the position, on the index list, of a vertex that can be clipped.
    /// Returns -1 when no vertex qualifies.
    /// </summary>
    static int FindEar(float[] pts, List<int> indices, double epsilon, bool allowBoundaryContact)
    {
        var count = indices.Count;

        for (int i = 0; i < count; i++)
        {
            var prev = indices[(i + count - 1) % count];
            var curr = indices[i];
            var next = indices[(i + 1) % count];

            var (ax, ay) = Point(pts, prev);
            var (bx, by) = Point(pts, curr);
            var (cx, cy) = Point(pts, next);

            // Reflex or collinear on a counterclockwise polygon.
            if (Cross(ax, ay, bx, by, cx, cy) <= epsilon)
                continue;

            if (IsEmpty(pts, indices, prev, curr, next, epsilon, allowBoundaryContact))
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Get if no vertex of the polygon lies inside the triangule (prev, curr, next).
    /// When allowBoundaryContact is true only strictly interior vertices block it.
    /// </summary>
    static bool IsEmpty(
        float[] pts, List<int> indices,
        int prev, int curr, int next,
        double epsilon, bool allowBoundaryContact)
    {
        var (ax, ay) = Point(pts, prev);
        var (bx, by) = Point(pts, curr);
        var (cx, cy) = Point(pts, next);
        var limit = allowBoundaryContact ? epsilon : -epsilon;

        foreach (var index in indices)
        {
            if (index == prev || index == curr || index == next)
                continue;

            var (px, py) = Point(pts, index);

            if (Cross(ax, ay, bx, by, px, py) <= limit)
                continue;

            if (Cross(bx, by, cx, cy, px, py) <= limit)
                continue;

            if (Cross(cx, cy, ax, ay, px, py) <= limit)
                continue;

            return false;
        }

        return true;
    }

    /// <summary>
    /// Get the position of the vertex with the largest turn to the left.
    /// Used as a last resort so the clipping always progresses.
    /// </summary>
    static int SharpestVertex(float[] pts, List<int> indices)
    {
        var count = indices.Count;
        var best = 0;
        var bestCross = double.MinValue;

        for (int i = 0; i < count; i++)
        {
            var (ax, ay) = Point(pts, indices[(i + count - 1) % count]);
            var (bx, by) = Point(pts, indices[i]);
            var (cx, cy) = Point(pts, indices[(i + 1) % count]);

            var cross = Cross(ax, ay, bx, by, cx, cy);
            if (cross <= bestCross)
                continue;

            bestCross = cross;
            best = i;
        }

        return best;
    }

    /// <summary>
    /// Drop vertices repeated in sequence, including the wrap around pair,
    /// because they cannot define an orientation.
    /// </summary>
    static List<int> DropRepeatedVertices(float[] pts, int vertexCount)
    {
        var indices = new List<int>(vertexCount);

        for (int i = 0; i < vertexCount; i++)
        {
            if (indices.Count > 0 && SamePoint(pts, indices[^1], i))
                continue;

            indices.Add(i);
        }

        while (indices.Count > 1 && SamePoint(pts, indices[0], indices[^1]))
            indices.RemoveAt(indices.Count - 1);

        return indices;
    }

    /// <summary>
    /// Drop vertices aligned with both neighbours. They add no area and could
    /// only become triangules with no area, so removing them also spares the
    /// GPU from vertices that rasterize to nothing.
    /// </summary>
    static void DropCollinearVertices(float[] pts, List<int> indices, double epsilon)
    {
        var removedAny = true;

        while (removedAny && indices.Count > 3)
        {
            removedAny = false;

            for (int i = 0; i < indices.Count && indices.Count > 3; )
            {
                var count = indices.Count;
                var (ax, ay) = Point(pts, indices[(i + count - 1) % count]);
                var (bx, by) = Point(pts, indices[i]);
                var (cx, cy) = Point(pts, indices[(i + 1) % count]);

                if (Math.Abs(Cross(ax, ay, bx, by, cx, cy)) > epsilon)
                {
                    i++;
                    continue;
                }

                indices.RemoveAt(i);
                removedAny = true;
            }
        }
    }

    /// <summary>
    /// Get the signed area of the polygon. Positive on counterclockwise input.
    /// </summary>
    static double SignedArea(float[] pts, List<int> indices)
    {
        double sum = 0;

        for (int i = 0; i < indices.Count; i++)
        {
            var (x0, y0) = Point(pts, indices[i]);
            var (x1, y1) = Point(pts, indices[(i + 1) % indices.Count]);
            sum += x0 * y1 - x1 * y0;
        }

        return sum / 2;
    }

    /// <summary>
    /// Get a tolerance for area comparisons scaled by the size of the polygon,
    /// so unit sized and pixel sized data behave the same way.
    /// </summary>
    static double AreaEpsilon(float[] pts, List<int> indices)
    {
        double minX = double.MaxValue, maxX = double.MinValue;
        double minY = double.MaxValue, maxY = double.MinValue;

        foreach (var index in indices)
        {
            var (x, y) = Point(pts, index);
            minX = Math.Min(minX, x);
            maxX = Math.Max(maxX, x);
            minY = Math.Min(minY, y);
            maxY = Math.Max(maxY, y);
        }

        var scale = Math.Max(maxX - minX, maxY - minY);
        return scale * scale * 1e-12;
    }

    /// <summary>
    /// Add the triangule formed by the vertex on a position and its neighbours.
    /// Triangules with no area are skipped: alignments can appear while the
    /// clipping runs and they would only cost vertices that draw nothing.
    /// </summary>
    static void AddTriangule(float[] pts, List<float> triangules, List<int> indices, int position, double epsilon)
    {
        var count = indices.Count;
        var prev = indices[(position + count - 1) % count];
        var curr = indices[position];
        var next = indices[(position + 1) % count];

        var (ax, ay) = Point(pts, prev);
        var (bx, by) = Point(pts, curr);
        var (cx, cy) = Point(pts, next);

        if (Math.Abs(Cross(ax, ay, bx, by, cx, cy)) <= epsilon)
            return;

        AddVertex(pts, triangules, prev);
        AddVertex(pts, triangules, curr);
        AddVertex(pts, triangules, next);
    }

    static void AddVertex(float[] pts, List<float> triangules, int index)
    {
        triangules.Add(pts[3 * index + 0]);
        triangules.Add(pts[3 * index + 1]);
        triangules.Add(pts[3 * index + 2]);
    }

    static (double x, double y) Point(float[] pts, int index)
        => (pts[3 * index + 0], pts[3 * index + 1]);

    static bool SamePoint(float[] pts, int i, int j)
        => pts[3 * i + 0] == pts[3 * j + 0]
        && pts[3 * i + 1] == pts[3 * j + 1];

    /// <summary>
    /// Get a positive value when the points a, b and c turn to the left.
    /// </summary>
    static double Cross(
        double ax, double ay,
        double bx, double by,
        double cx, double cy)
        => (bx - ax) * (cy - ay) - (by - ay) * (cx - ax);
}
