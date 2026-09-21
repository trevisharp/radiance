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
        => Triangulate([ pts ]);

    /// <summary>
    /// Get a triangulation of a polygon with holes. The contour and every hole
    /// are (x, y, z) sequences and may be given in any orientation.
    /// </summary>
    public static float[] PlanarPolygonTriangulation(float[] contour, float[][] holes)
        => Triangulate([ contour, ..holes ]);

    /// <summary>
    /// Get a triangulation of a shape described by closed rings of (x, y, z)
    /// points, in any orientation and in any order. A ring nested inside an odd
    /// number of other rings is a hole, which is how a glyph arrives here: one
    /// call, many contours, some of them counters. Each filled ring is sewn to
    /// its holes by a bridge and then ear clipped, and all the triangules are
    /// returned together, counterclockwise on the xy plane.
    /// </summary>
    public static float[] Triangulate(float[][] rings)
        => Triangulate(rings, FillRule.Nesting);

    /// <summary>
    /// Get a triangulation of a shape described by closed rings of (x, y, z)
    /// points, in any order. Each filled ring is sewn to its holes by a bridge
    /// and then ear clipped, and all the triangules are returned together,
    /// counterclockwise on the xy plane.
    /// </summary>
    public static float[] Triangulate(float[][] rings, FillRule rule)
    {
        if (rings.Length == 0)
            return [];

        var pts = MergePoints(rings);
        var epsilon = AreaEpsilon(pts);

        var indexRings = BuildRings(pts, rings, epsilon);
        var boxes = MeasureBoxes(pts, indexRings);
        var holeFlags = rule == FillRule.Winding
            ? ClassifyByWinding(pts, indexRings)
            : ClassifyByNesting(pts, indexRings, boxes);

        // Each hole is matched to the ring it belongs to once, up front.
        // Searching for it inside the loop below made the whole thing cubic on
        // the number of rings, which a page of text reaches easily.
        var owners = FindOwners(pts, indexRings, holeFlags, boxes);

        var triangules = new List<float>();

        for (int i = 0; i < indexRings.Count; i++)
        {
            if (indexRings[i].Count < 3 || holeFlags[i])
                continue;

            var outer = new List<int>(indexRings[i]);
            Orient(pts, outer, counterclockwise: true);

            var holes = CollectHoles(indexRings, owners, i, pts);
            while (holes.Count > 0)
                BridgeHole(pts, outer, TakeRightmostRing(pts, holes), holes, epsilon);

            EarClipping(pts, outer, triangules, epsilon);
        }

        return triangules.Count > 0 ? [ ..triangules ] : rings[0];
    }

    /// <summary>
    /// Decide what is a hole by nesting: a ring inside an odd number of other
    /// rings is a hole. Friendly for hand written shapes, where nobody wants
    /// to think about which way around a contour goes.
    /// </summary>
    static bool[] ClassifyByNesting(float[] pts, List<List<int>> rings, Box[] boxes)
    {
        var holes = new bool[rings.Count];

        for (int i = 0; i < rings.Count; i++)
        {
            if (rings[i].Count < 3)
                continue;

            var depth = 0;
            for (int j = 0; j < rings.Count; j++)
            {
                if (i == j || rings[j].Count < 3)
                    continue;

                if (!boxes[j].Contains(boxes[i]))
                    continue;

                if (IsRingInside(pts, rings[i], rings[j]))
                    depth++;
            }

            holes[i] = depth % 2 == 1;
        }

        return holes;
    }

    /// <summary>
    /// Get the bounding box of every ring, used to skip the point in polygon
    /// test between rings that cannot possibly contain one another.
    /// </summary>
    static Box[] MeasureBoxes(float[] pts, List<List<int>> rings)
    {
        var boxes = new Box[rings.Count];

        for (int i = 0; i < rings.Count; i++)
        {
            double minX = double.MaxValue, maxX = double.MinValue;
            double minY = double.MaxValue, maxY = double.MinValue;

            foreach (var index in rings[i])
            {
                var (x, y) = Point(pts, index);
                minX = Math.Min(minX, x);
                maxX = Math.Max(maxX, x);
                minY = Math.Min(minY, y);
                maxY = Math.Max(maxY, y);
            }

            boxes[i] = new Box(minX, minY, maxX, maxY);
        }

        return boxes;
    }

    readonly record struct Box(double MinX, double MinY, double MaxX, double MaxY)
    {
        public readonly double Area => (MaxX - MinX) * (MaxY - MinY);

        public readonly bool Contains(Box other)
            => other.MinX >= MinX && other.MaxX <= MaxX
            && other.MinY >= MinY && other.MaxY <= MaxY;
    }

    /// <summary>
    /// For every hole, get the smallest filled ring that contains it, or -1
    /// when it sits outside everything.
    /// </summary>
    static int[] FindOwners(float[] pts, List<List<int>> rings, bool[] holeFlags, Box[] boxes)
    {
        var owners = new int[rings.Count];

        for (int i = 0; i < rings.Count; i++)
        {
            owners[i] = -1;

            if (!holeFlags[i] || rings[i].Count < 3)
                continue;

            var ownerArea = double.MaxValue;

            for (int j = 0; j < rings.Count; j++)
            {
                if (i == j || holeFlags[j] || rings[j].Count < 3)
                    continue;

                // Both tests are there to keep the expensive one rare: a
                // candidate bigger than the best so far cannot win anyway, and
                // a box that does not cover the hole cannot contain it.
                if (boxes[j].Area >= ownerArea || !boxes[j].Contains(boxes[i]))
                    continue;

                if (!IsRingInside(pts, rings[i], rings[j]))
                    continue;

                ownerArea = boxes[j].Area;
                owners[i] = j;
            }
        }

        return owners;
    }

    /// <summary>
    /// Decide what is a hole by the direction the ring runs. This is the rule
    /// font files are built on, and it is the only one that survives contours
    /// that overlap instead of nesting: the cedilla of a c is a filled shape
    /// crossing the letter, not a hole, and only its direction says so.
    /// </summary>
    static bool[] ClassifyByWinding(float[] pts, List<List<int>> rings)
    {
        var holes = new bool[rings.Count];

        for (int i = 0; i < rings.Count; i++)
            holes[i] = rings[i].Count >= 3 && SignedArea(pts, rings[i]) > 0;

        return holes;
    }

    /// <summary>
    /// Get the holes that belong to the filled ring on a position.
    /// </summary>
    static List<List<int>> CollectHoles(List<List<int>> rings, int[] owners, int outer, float[] pts)
    {
        var holes = new List<List<int>>();

        for (int i = 0; i < rings.Count; i++)
        {
            if (owners[i] != outer)
                continue;

            var hole = new List<int>(rings[i]);
            Orient(pts, hole, counterclockwise: false);
            holes.Add(hole);
        }

        return holes;
    }

    /// <summary>
    /// Get if a ring lies inside another one. Rings never cross each other on
    /// well formed input, so a single point decides it. The middle of an edge
    /// is used because it is less likely than a vertex to fall exactly over
    /// the other ring.
    /// </summary>
    static bool IsRingInside(float[] pts, List<int> ring, List<int> other)
    {
        var (ax, ay) = Point(pts, ring[0]);
        var (bx, by) = Point(pts, ring[1]);

        return Contains(pts, other, (ax + bx) / 2, (ay + by) / 2);
    }

    /// <summary>
    /// Turn a ring to the wanted orientation.
    /// </summary>
    static void Orient(float[] pts, List<int> ring, bool counterclockwise)
    {
        if (SignedArea(pts, ring) < 0 == counterclockwise)
            ring.Reverse();
    }

    /// <summary>
    /// Build the clean index ring of every input ring.
    /// </summary>
    static List<List<int>> BuildRings(float[] pts, float[][] rings, double epsilon)
    {
        var result = new List<List<int>>(rings.Length);
        var offset = 0;

        foreach (var ring in rings)
        {
            result.Add(BuildRing(pts, offset, ring.Length / 3, epsilon));
            offset += ring.Length / 3;
        }

        return result;
    }

    /// <summary>
    /// Join the contour and every hole on a single point array, so that all the
    /// rings can be described by indexes over the same data.
    /// </summary>
    static float[] MergePoints(float[][] rings)
    {
        if (rings.Length == 1)
            return rings[0];

        var size = 0;
        foreach (var ring in rings)
            size += ring.Length;

        var pts = new float[size];
        var offset = 0;

        foreach (var ring in rings)
        {
            Array.Copy(ring, 0, pts, offset, ring.Length);
            offset += ring.Length;
        }

        return pts;
    }

    /// <summary>
    /// Build a clean index ring over a slice of the point array: drops repeated
    /// and aligned vertices.
    /// </summary>
    static List<int> BuildRing(float[] pts, int offset, int count, double epsilon)
    {
        var ring = DropRepeatedVertices(pts, offset, count);
        if (ring.Count < 3)
            return ring;

        DropCollinearVertices(pts, ring, epsilon);
        return ring;
    }

    /// <summary>
    /// Remove and return the ring whose rightmost vertex is the rightmost of all.
    /// </summary>
    static List<int> TakeRightmostRing(float[] pts, List<List<int>> rings)
    {
        var best = 0;
        var bestX = double.MinValue;

        for (int i = 0; i < rings.Count; i++)
        {
            var (x, _) = Point(pts, rings[i][RightmostPosition(pts, rings[i])]);
            if (x <= bestX)
                continue;

            bestX = x;
            best = i;
        }

        var ring = rings[best];
        rings.RemoveAt(best);
        return ring;
    }

    static int RightmostPosition(float[] pts, List<int> ring)
    {
        var best = 0;
        var bestX = double.MinValue;

        for (int i = 0; i < ring.Count; i++)
        {
            var (x, _) = Point(pts, ring[i]);
            if (x <= bestX)
                continue;

            bestX = x;
            best = i;
        }

        return best;
    }

    /// <summary>
    /// Sew a hole into the contour through a bridge: a segment between a hole
    /// vertex and a contour vertex that crosses no edge and stays on the filled
    /// side. The hole is walked in its own direction and the bridge is travelled
    /// twice, which turns contour and hole into a single ring.
    /// </summary>
    static void BridgeHole(
        float[] pts, List<int> outer, List<int> hole,
        List<List<int>> pendingHoles, double epsilon)
    {
        var candidates = CandidateBridges(pts, outer, hole);

        // Two traps are avoided on the first pass. A bridge whose middle lands
        // on a border leaves the inside test undecidable. And a bridge anchored
        // on a vertex another bridge already used pinches the ring at a single
        // point, where no triangule can be cut: the letter B hits this, both
        // counters want the same corner of the contour.
        if (TryBridge(pts, outer, hole, pendingHoles, candidates, epsilon, careful: true))
            return;

        // Nothing clean exists: take a bridge that at least crosses no edge,
        // which still beats dropping the hole and filling it solid.
        TryBridge(pts, outer, hole, pendingHoles, candidates, epsilon, careful: false);
    }

    static bool TryBridge(
        float[] pts, List<int> outer, List<int> hole, List<List<int>> pendingHoles,
        (int hole, int outer)[] candidates, double epsilon, bool careful)
    {
        var used = careful ? RepeatedIndices(outer) : null;

        foreach (var (holePosition, outerPosition) in candidates)
        {
            var from = outer[outerPosition];
            var to = hole[holePosition];

            if (used is not null && used.Contains(from))
                continue;

            if (!IsBridgeClear(pts, outer, hole, pendingHoles, from, to))
                continue;

            if (!IsBridgeInside(pts, outer, hole, pendingHoles, from, to, epsilon, careful))
                continue;

            var sewed = new List<int>(hole.Count + 2);
            for (int i = 0; i <= hole.Count; i++)
                sewed.Add(hole[(holePosition + i) % hole.Count]);
            sewed.Add(from);

            outer.InsertRange(outerPosition + 1, sewed);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Get the vertices that already appear more than once on the ring, which
    /// are exactly the anchors of the bridges sewn so far.
    /// </summary>
    static HashSet<int> RepeatedIndices(List<int> ring)
    {
        var seen = new HashSet<int>(ring.Count);
        var repeated = new HashSet<int>();

        foreach (var index in ring)
        {
            if (!seen.Add(index))
                repeated.Add(index);
        }

        return repeated;
    }

    /// <summary>
    /// Get the candidate vertex pairs ordered by distance, so the shortest
    /// bridge that works is the one used.
    /// </summary>
    static (int hole, int outer)[] CandidateBridges(float[] pts, List<int> outer, List<int> hole)
    {
        var candidates = new (int hole, int outer)[hole.Count * outer.Count];
        var distances = new double[candidates.Length];

        for (int i = 0, k = 0; i < hole.Count; i++)
        {
            var (hx, hy) = Point(pts, hole[i]);

            for (int j = 0; j < outer.Count; j++, k++)
            {
                var (ox, oy) = Point(pts, outer[j]);
                candidates[k] = (i, j);
                distances[k] = (hx - ox) * (hx - ox) + (hy - oy) * (hy - oy);
            }
        }

        Array.Sort(distances, candidates);
        return candidates;
    }

    /// <summary>
    /// Get if the bridge crosses no edge of the contour nor of any hole.
    /// </summary>
    static bool IsBridgeClear(
        float[] pts, List<int> outer, List<int> hole,
        List<List<int>> pendingHoles, int from, int to)
    {
        if (CrossesRing(pts, outer, from, to))
            return false;

        if (CrossesRing(pts, hole, from, to))
            return false;

        foreach (var other in pendingHoles)
        {
            if (CrossesRing(pts, other, from, to))
                return false;
        }

        return true;
    }

    static bool CrossesRing(float[] pts, List<int> ring, int from, int to)
    {
        var (ax, ay) = Point(pts, from);
        var (bx, by) = Point(pts, to);

        for (int i = 0; i < ring.Count; i++)
        {
            var u = ring[i];
            var v = ring[(i + 1) % ring.Count];

            if (u == from || u == to || v == from || v == to)
                continue;

            var (cx, cy) = Point(pts, u);
            var (dx, dy) = Point(pts, v);

            if (SegmentsCross(ax, ay, bx, by, cx, cy, dx, dy))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Get if two segments cross each other on a single interior point.
    /// </summary>
    static bool SegmentsCross(
        double ax, double ay, double bx, double by,
        double cx, double cy, double dx, double dy)
    {
        var d1 = Cross(ax, ay, bx, by, cx, cy);
        var d2 = Cross(ax, ay, bx, by, dx, dy);
        var d3 = Cross(cx, cy, dx, dy, ax, ay);
        var d4 = Cross(cx, cy, dx, dy, bx, by);

        return ((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0))
            && ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0));
    }

    /// <summary>
    /// Get if the middle of the bridge falls on the filled part of the polygon:
    /// inside the contour and outside every hole not sewn yet.
    /// </summary>
    static bool IsBridgeInside(
        float[] pts, List<int> outer, List<int> hole,
        List<List<int>> pendingHoles, int from, int to,
        double epsilon, bool avoidBorder)
    {
        var (ax, ay) = Point(pts, from);
        var (bx, by) = Point(pts, to);
        var mx = (ax + bx) / 2;
        var my = (ay + by) / 2;

        if (avoidBorder && TouchesBorder(pts, outer, hole, pendingHoles, mx, my, epsilon))
            return false;

        if (!Contains(pts, outer, mx, my))
            return false;

        if (Contains(pts, hole, mx, my))
            return false;

        foreach (var other in pendingHoles)
        {
            if (Contains(pts, other, mx, my))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Get if a point lies over an edge of any ring, where a point in or out
    /// test cannot answer.
    /// </summary>
    static bool TouchesBorder(
        float[] pts, List<int> outer, List<int> hole,
        List<List<int>> pendingHoles, double px, double py, double epsilon)
    {
        if (OnRing(pts, outer, px, py, epsilon) || OnRing(pts, hole, px, py, epsilon))
            return true;

        foreach (var other in pendingHoles)
        {
            if (OnRing(pts, other, px, py, epsilon))
                return true;
        }

        return false;
    }

    static bool OnRing(float[] pts, List<int> ring, double px, double py, double epsilon)
    {
        for (int i = 0; i < ring.Count; i++)
        {
            var (ax, ay) = Point(pts, ring[i]);
            var (bx, by) = Point(pts, ring[(i + 1) % ring.Count]);

            if (Math.Abs(Cross(ax, ay, bx, by, px, py)) > epsilon)
                continue;

            if (px < Math.Min(ax, bx) || px > Math.Max(ax, bx))
                continue;

            if (py < Math.Min(ay, by) || py > Math.Max(ay, by))
                continue;

            return true;
        }

        return false;
    }

    /// <summary>
    /// Even odd test of a point against a ring.
    /// </summary>
    static bool Contains(float[] pts, List<int> ring, double px, double py)
    {
        var inside = false;

        for (int i = 0, j = ring.Count - 1; i < ring.Count; j = i++)
        {
            var (xi, yi) = Point(pts, ring[i]);
            var (xj, yj) = Point(pts, ring[j]);

            if (yi > py == yj > py)
                continue;

            if (px < (xj - xi) * (py - yi) / (yj - yi) + xi)
                inside = !inside;
        }

        return inside;
    }

    /// <summary>
    /// Clip the ring one ear at a time until only a triangule remains.
    /// A ring that gets stuck is split and both halves go back on the queue,
    /// so the work always shrinks and the loop always ends.
    /// </summary>
    static void EarClipping(float[] pts, List<int> ring, List<float> triangules, double epsilon)
    {
        var pending = new Stack<List<int>>();
        pending.Push(ring);

        while (pending.Count > 0)
            ClipRing(pts, pending.Pop(), triangules, epsilon, pending);
    }

    static void ClipRing(float[] pts, List<int> indices, List<float> triangules, double epsilon, Stack<List<int>> pending)
    {
        while (indices.Count > 3)
        {
            // A vertex is an ear when it is convex and no reflex vertex of the
            // ring falls inside or on the triangule it forms with its
            // neighbours. Only reflex vertices can block an ear, and the border
            // counts: a bridge puts two copies of a vertex on the ring and
            // cutting through one of them would cross the slit.
            var ear = FindEar(pts, indices, epsilon, reflexOnly: true);

            // Degenerated input can leave no ear under that rule. Accepting any
            // vertex as long as nothing is strictly inside still gives a usable
            // cut instead of giving up.
            if (ear < 0)
                ear = FindEar(pts, indices, epsilon, reflexOnly: false);

            // A bridge can leave a run of aligned vertices on the ring, and an
            // aligned vertex is never an ear and blocks every candidate that
            // touches it. Dropping one is always safe, it carries no area.
            if (ear < 0)
                ear = FindAligned(pts, indices, epsilon);

            // Still stuck: the ring probably touches itself, which happens when
            // two bridges reach the same vertex. Cutting the ring in two at
            // that point separates the pinch into two ordinary rings.
            if (ear < 0 && TrySplit(pts, indices, pending))
                return;

            // Only reached by self intersecting or fully degenerated input,
            // where no real ear exists. Clipping the sharpest vertex keeps
            // the output finite instead of looping forever.
            if (ear < 0)
                ear = SharpestVertex(pts, indices);

            AddTriangule(pts, triangules, indices, ear, epsilon);
            indices.RemoveAt(ear);
        }

        if (indices.Count == 3)
            AddTriangule(pts, triangules, indices, 1, epsilon);
    }

    /// <summary>
    /// Split a ring that visits the same point twice into the two rings that
    /// meet there. Returns false when the ring never touches itself.
    /// </summary>
    static bool TrySplit(float[] pts, List<int> ring, Stack<List<int>> pending)
    {
        for (int i = 0; i < ring.Count; i++)
        {
            for (int j = i + 2; j < ring.Count; j++)
            {
                if (i == 0 && j == ring.Count - 1)
                    continue;

                if (!SamePoint(pts, ring[i], ring[j]))
                    continue;

                var first = ring.GetRange(i, j - i);

                var second = ring.GetRange(j, ring.Count - j);
                second.AddRange(ring.GetRange(0, i));

                if (first.Count >= 3)
                    pending.Push(first);

                if (second.Count >= 3)
                    pending.Push(second);

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Find the position, on the index list, of a vertex that can be clipped.
    /// Returns -1 when no vertex qualifies.
    /// </summary>
    static int FindEar(float[] pts, List<int> indices, double epsilon, bool reflexOnly)
    {
        var count = indices.Count;

        for (int i = 0; i < count; i++)
        {
            if (IsReflex(pts, indices, i, epsilon))
                continue;

            if (IsEmpty(pts, indices, i, epsilon, reflexOnly))
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Get if the vertex on a position turns to the right, or does not turn at
    /// all, on a counterclockwise ring.
    /// </summary>
    static bool IsReflex(float[] pts, List<int> indices, int position, double epsilon)
    {
        var count = indices.Count;
        var (ax, ay) = Point(pts, indices[(position + count - 1) % count]);
        var (bx, by) = Point(pts, indices[position]);
        var (cx, cy) = Point(pts, indices[(position + 1) % count]);

        return Cross(ax, ay, bx, by, cx, cy) <= epsilon;
    }

    /// <summary>
    /// Get if the triangule clipped at a position is free of blocking vertices.
    /// With reflexOnly, a vertex blocks when it is reflex and lies inside or on
    /// the triangule; otherwise only strictly interior vertices block.
    /// </summary>
    static bool IsEmpty(float[] pts, List<int> indices, int position, double epsilon, bool reflexOnly)
    {
        var count = indices.Count;
        var previous = (position + count - 1) % count;
        var next = (position + 1) % count;

        var (ax, ay) = Point(pts, indices[previous]);
        var (bx, by) = Point(pts, indices[position]);
        var (cx, cy) = Point(pts, indices[next]);
        var limit = reflexOnly ? -epsilon : epsilon;

        // Positions are compared instead of indexes because a bridge makes the
        // same vertex appear twice on the ring.
        for (int i = 0; i < count; i++)
        {
            if (i == previous || i == position || i == next)
                continue;

            var (px, py) = Point(pts, indices[i]);

            if (Cross(ax, ay, bx, by, px, py) <= limit)
                continue;

            if (Cross(bx, by, cx, cy, px, py) <= limit)
                continue;

            if (Cross(cx, cy, ax, ay, px, py) <= limit)
                continue;

            if (reflexOnly && !IsReflex(pts, indices, i, epsilon))
                continue;

            return false;
        }

        return true;
    }

    /// <summary>
    /// Find a vertex aligned with both neighbours, which can be dropped
    /// without changing the shape.
    /// </summary>
    static int FindAligned(float[] pts, List<int> indices, double epsilon)
    {
        var count = indices.Count;

        for (int i = 0; i < count; i++)
        {
            var (ax, ay) = Point(pts, indices[(i + count - 1) % count]);
            var (bx, by) = Point(pts, indices[i]);
            var (cx, cy) = Point(pts, indices[(i + 1) % count]);

            if (Math.Abs(Cross(ax, ay, bx, by, cx, cy)) <= epsilon)
                return i;
        }

        return -1;
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
    /// Remove vertices repeated in sequence, including the wrap around pair,
    /// because they cannot define an orientation.
    /// </summary>
    static List<int> DropRepeatedVertices(float[] pts, int offset, int count)
    {
        var indices = new List<int>(count);

        for (int i = 0; i < count; i++)
        {
            if (indices.Count > 0 && SamePoint(pts, indices[^1], offset + i))
                continue;

            indices.Add(offset + i);
        }

        while (indices.Count > 1 && SamePoint(pts, indices[0], indices[^1]))
            indices.RemoveAt(indices.Count - 1);

        return indices;
    }

    /// <summary>
    /// Remove vertices aligned with both neighbours. They add no area and could
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
    /// Get the signed area of a ring. Positive when counterclockwise.
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
    static double AreaEpsilon(float[] pts)
    {
        double minX = double.MaxValue, maxX = double.MinValue;
        double minY = double.MaxValue, maxY = double.MinValue;

        for (int i = 0; i < pts.Length / 3; i++)
        {
            var (x, y) = Point(pts, i);
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
        var previous = indices[(position + count - 1) % count];
        var current = indices[position];
        var next = indices[(position + 1) % count];

        var (ax, ay) = Point(pts, previous);
        var (bx, by) = Point(pts, current);
        var (cx, cy) = Point(pts, next);

        if (Math.Abs(Cross(ax, ay, bx, by, cx, cy)) <= epsilon)
            return;

        AddVertex(pts, triangules, previous);
        AddVertex(pts, triangules, current);
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
