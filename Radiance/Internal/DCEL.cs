/* Author:  Leonardo Trevisan Silio
 * Date:    27/03/2025
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Radiance.Internal;

/// <summary>
/// Represents a Double Connected Edge List.
/// </summary>
public ref struct DCEL
{
    int nextEdgeId = 0;
    int nextFaceId = 0;
    readonly Span<PlanarVertex> OriginalSource;
    public readonly int Length;
    public readonly List<HalfEdge> Edges = [];
    public readonly Dictionary<int, List<HalfEdge>> VertexEdges = [];
    public readonly Dictionary<int, List<int>> Faces = [];
    public readonly Dictionary<int, List<HalfEdge>> FacesEdges = [];

    public DCEL(Span<PlanarVertex> source, int[] points)
    {
        OriginalSource = source;
        Length = points.Length;

        int face = CreateFace();
        List<int> faceVertexes = Faces[face];
        List<HalfEdge> faceEdges = FacesEdges[face];

        for (int j = 0; j < points.Length; j++)
            faceVertexes.Add(points[j]);

        HalfEdge fst, prv;
        fst = prv = CreateEdge(
            points[0],
            points[1],
            face
        );

        int i = 1;
        while (i < points.Length - 1)
        {
            var crr = CreateEdge(
                points[i],
                points[i + 1],
                face
            );
            faceEdges.Add(crr);
            crr.SetPrevious(prv);

            prv = crr;
            i++;
        }

        var lst = CreateEdge(
            points[i], 
            points[0],
            face
        );
        faceEdges.Add(lst);
        lst.SetPrevious(prv);
        lst.SetNext(fst);
    }

    public DCEL(Span<PlanarVertex> points)
    {
        OriginalSource = points;
        Length = points.Length;

        int face = CreateFace();
        List<int> faceVertexes = Faces[face];
        List<HalfEdge> faceEdges = FacesEdges[face];

        for (int j = 0; j < points.Length; j++)
            faceVertexes.Add(j);

        HalfEdge fst, prv;
        fst = prv = CreateEdge(0, 1, face);

        int i = 1;
        while (i < points.Length - 1)
        {
            var crr = CreateEdge(
                i,
                i + 1,
                face
            );
            faceEdges.Add(crr);
            crr.SetPrevious(prv);

            prv = crr;
            i++;
        }

        var lst = CreateEdge(
            i, 
            0,
            face
        );
        faceEdges.Add(lst);
        lst.SetPrevious(prv);
        lst.SetNext(fst);
    }

    /// <summary>
    /// Receiving 2 ids for vertex return if them are connected.
    /// </summary>
    public readonly bool IsConnected(int v, int u)
        => VertexEdges[v].Any(e => e.To == u) 
        || VertexEdges[u].Any(e => e.To == v);

    /// <summary>
    /// Add a Edge between two vertex.
    /// </summary>
    public bool Connect(int v, int u)
    {
        if (v == u)
            return false;
        
        var faceId = GetSharedFace(v, u);
        if (faceId is null)
            return false;
        
        if (IsConnected(v, u))
            return false;
        
        Console.WriteLine($"Connect({v}, {u})");
        
        var currFace = faceId.Value;
        var othrFace = CreateFace();
        
        List<int> currPoints = [];
        List<int> othrPoits = [];

        List<HalfEdge> currEdges = [];
        List<HalfEdge> othrEdges = [];

        var sharedEdges = GetFaceEdgeList(currFace);
        var fstEdge = sharedEdges[0];
        var edge = fstEdge;
        
        do
        {
            var vertex = edge.To;
            edge.FaceId = currFace;
            currPoints.Add(vertex);
            currEdges.Add(edge);
            edge = edge.Next!;

            if (vertex == v)
            {
                currPoints.Add(u);
                (currFace, othrFace) = (othrFace, currFace);
                (currPoints, othrPoits) = (othrPoits, currPoints);
                (currEdges, othrEdges) = (othrEdges, currEdges);
            }

            if (vertex == u)
            {
                currPoints.Add(v);
                (currFace, othrFace) = (othrFace, currFace);
                (currPoints, othrPoits) = (othrPoits, currPoints);
                (currEdges, othrEdges) = (othrEdges, currEdges);
            }

        } while (edge != fstEdge);

        Faces[currFace] = currPoints;
        Faces[othrFace] = othrPoits;
        FacesEdges[currFace] = currEdges;
        FacesEdges[othrFace] = othrEdges;

        if (!currEdges.Any(x => x.To == v))
            (v, u) = (u, v);
        var e1 = CreateEdge(v, u, currFace);
        foreach (var e in currEdges)
        {
            if (e == e1)
                continue;

            if (e.To == v)
            {
                e.SetNext(e1);
                continue;
            }
            
            if (e.From == u)
            {
                e.SetPrevious(e1);
                continue;
            }
        }

        var e2 = CreateEdge(u, v, othrFace);
        foreach (var e in othrEdges)
        {
            if (e == e2)
                continue;
            
            if (e.To == u)
            {
                e.SetNext(e2);
                continue;
            }
            
            if (e.From == v)
            {
                e.SetPrevious(e2);
                continue;
            }
        }

        return true;
    }

    /// <summary>
    /// Return true if two vertices can connect with a line
    /// inside the polygon.
    /// </summary>
    public readonly bool CanInternalConnect(int vid, int uid)
    {
        ref var v = ref GetVertex(vid);
        ref var u = ref GetVertex(uid);

        foreach (var edge in Edges)
        {
            if (edge.From == vid || edge.To == vid || edge.From == uid || edge.To == uid)
                continue;

            ref var v2 = ref GetVertex(edge.From);
            ref var u2 = ref GetVertex(edge.To);

            if (Intersect(ref v, ref u, ref v2, ref u2))
                return false;
        }

        var count = 0;
        var mid = ((v.Xp + u.Xp) / 2, (v.Yp + u.Yp) / 2);
        foreach (var edge in Edges)
        {
            ref var v2 = ref GetVertex(edge.From);
            ref var u2 = ref GetVertex(edge.To);

            if (RayIntersect(ref v2, ref u2, mid.Item1, mid.Item2))
                count++;
        }
        
        return count % 2 == 1;
    }

    /// <summary>
    /// Get two hash set of the left and right chain over a sweep line.
    /// </summary>
    public readonly (HashSet<int> left, HashSet<int> right) GetChains(SweepLine sweepLine)
    {
        var top = sweepLine[0].Id;
        var bottom = sweepLine[^1].Id;
        var current = top;

        HashSet<int> leftChain = [ top, bottom ];
        HashSet<int> rightChain = [ ];

        while (current != bottom)
        {
            leftChain.Add(current);
            current = VertexEdges[current][0].To;
        }
        
        current = VertexEdges[current][0].To;
        while (current != top)
        {
            rightChain.Add(current);
            current = VertexEdges[current][0].To;
        }

        return (rightChain, leftChain);
    }

    /// <summary>
    /// Discover the type of the vertex with specific id.
    /// </summary>
    public readonly VertexType DiscoverType(int v)
    {
        var edges = VertexEdges[v];
        var edge = edges[0];
        ref var self = ref GetVertex(v);
        ref var e1 = ref GetVertex(edge.To);
        ref var e2 = ref GetVertex(edge.Previous!.From);
        
        if (over(ref self, ref e1) && over(ref self, ref e2))
            return Left(ref e1, ref self, ref e2) > 0 ?
                VertexType.Split : VertexType.Start;
        
        if (over(ref e1, ref self) && over(ref e2, ref self))
            return Left(ref e1, ref self, ref e2) > 0 ?
                VertexType.Merge : VertexType.End;

        return VertexType.Regular;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool over(ref PlanarVertex p, ref PlanarVertex q)
            => p.Yp > q.Yp || (p.Yp == q.Yp && p.Xp > q.Xp);
    }

    /// <summary>
    /// Find the left edge from a vertex. If are two left
    /// edges the algorithm choose the least y-axis. 
    /// </summary>
    public readonly int FindLeftEdge(int vertexId)
    {
        var vert = GetVertex(vertexId);
        var y = vert.Yp;
        var x = vert.Xp;

        int selected = -1;
        float bestX = float.MaxValue;

        foreach (var edge in Edges)
        {
            if (edge.From == vertexId)
                continue;
            
            if (edge.To == vertexId)
                continue;

            var v = GetVertex(edge.To);
            var x1 = v.Xp;
            var y1 = v.Yp;

            var u = GetVertex(edge.From);
            var x2 = u.Xp;
            var y2 = u.Yp;

            var between = y1 >= y && y > y2 || y2 >= y && y > y1;
            if (!between)
                continue;

            var minX = float.Min(x1, x2);
            if (minX > bestX)
                continue;
            
            if (Left(v.Id, vertexId, u.Id) < 0)
                continue;

            bestX = minX;
            selected = edge.Id;
        }
        
        return selected;
    }

    /// <summary>
    /// Filter DCEL considering some points of original source.
    /// </summary>
    public readonly DCEL ApplyFilter(int[] points)
        => new (OriginalSource, points);

    /// <summary>
    /// Get the face shader by two vertex
    /// with id 'v' and 'u'.
    /// </summary>
    public readonly int? GetSharedFace(int vid, int uid)
    {
        foreach (var (faceId, _) in Faces)
        {
            var edges = FacesEdges[faceId];
            bool hasV = false,
                 hasU = false;
            
            foreach (var edge in edges)
            {
                if (edge.From == vid)
                    hasV = true;
                
                if (edge.From == uid)
                    hasU = true;
            }

            if (hasV && hasU)
                return faceId;
        }

        return null;
    }

    /// <summary>
    /// Remove a random subpolygon and return a new DCEL.
    /// </summary>
    public readonly int[] RemoveSubPolygon()
    {
        var face = Faces.Keys.Last();
        var points = Faces[face];

        Faces.Remove(face);
        FacesEdges.Remove(face);

        return [ ..points ];
    }

    /// <summary>
    /// Returns true if the polygon lies to the right of vi.
    /// </summary>
    public readonly bool LiesOnRight(int vid)
    {
        return false;
    }

    /// <summary>
    /// Get a array of points.
    /// </summary>
    public readonly float[] ToArray()
    {
        List<float> values = [];
        foreach (var face in Faces)
        {
            foreach (var vertexId in face.Value)
            {
                ref var vertex = ref GetVertex(vertexId);
                values.Add(vertex.X);
                values.Add(vertex.Y);
                values.Add(vertex.Z);
            }
        }
        
        return [.. values];
    }

    /// <summary>
    /// Get a Planar Vertex by id.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ref PlanarVertex GetVertex(int id)
        => ref OriginalSource[id];

    /// <summary>
    /// Create a new empty face.
    /// </summary>
    int CreateFace()
    {
        var id = nextFaceId;
        nextFaceId++;

        Faces.Add(id, []);
        FacesEdges.Add(id, []);
        return id;
    }

    /// <summary>
    /// Create a new Edge between 'from' and 'to'
    /// on specific face. Do not create new face
    /// and do not keep face consistency.
    /// </summary>
    HalfEdge CreateEdge(int from, int to, int face)
    {
        var id = nextEdgeId;
        nextEdgeId++;

        var edge = new HalfEdge(id, from, to, face);
        var edges = GetEdgeList(from);
        edges.Add(edge);
        Edges.Add(edge);

        var faceEdges = GetFaceEdgeList(face);
        faceEdges.Add(edge);

        return edge;
    }

    /// <summary>
    /// Get, and init if needed, edges connect
    /// to a vertex with specific id.
    /// </summary>
    readonly List<HalfEdge> GetEdgeList(int id)
    {
        if (VertexEdges.TryGetValue(id, out var edges))
            return edges;
        
        edges = [];
        VertexEdges.Add(id, edges);
        return edges;
    }

    /// <summary>
    /// Get, and init if needed, edges in
    /// a specific face.
    /// </summary>
    readonly List<HalfEdge> GetFaceEdgeList(int id)
    {
        if (FacesEdges.TryGetValue(id, out var edges))
            return edges;
        
        edges = [];
        FacesEdges.Add(id, edges);
        return edges;
    }
    
    /// <summary>
    /// Apply left between points based on ther Ids.
    /// </summary>
    public readonly float Left(int pid, int qId, int rId)
    {
        ref var p = ref GetVertex(pid);
        ref var q = ref GetVertex(qId);
        ref var r = ref GetVertex(rId);
        return Left(ref p, ref q, ref r);
    }

    /// <summary>
    /// The left operation. https://en.wikipedia.org/wiki/Left_and_right_(algebra)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float Left(ref PlanarVertex p, ref PlanarVertex q, ref PlanarVertex r)
        => Left(p.Xp, p.Yp, q.Xp, q.Yp, r.Xp, r.Yp);
    
    /// <summary>
    /// The left operation. https://en.wikipedia.org/wiki/Left_and_right_(algebra)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float Left(float px, float py, float qx, float qy, float rx, float ry)
    {
        var vx = px - qx;
        var vy = py - qy;
        
        var ux = rx - qx;
        var uy = ry - qy;

        return vx * uy - ux * vy;
    }

    /// <summary>
    /// Returns true if two lines (p, pf) and (q, qf) intersects.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool Intersect(
        ref PlanarVertex p, ref PlanarVertex pf,
        ref PlanarVertex q, ref PlanarVertex qf
    )
    {
        // alfa * vx + px = beta * ux + qx
        // alfa * vy + py = beta * uy + qy
        // alfa = (beta * ux + qx - px) / vx
        // vy * (beta * ux + qx - px) / vx + py = beta * uy + qy
        // beta * ux * vy / vx - beta * uy = qy - py - (qx - px) * vy / vx
        // beta * (ux * vy / vx - uy) = qy - py - (qx - px) * vy / vx
        
        var vx = pf.Xp - p.Xp;
        var vy = pf.Yp - p.Yp;
        var ux = qf.Xp - q.Xp;
        var uy = qf.Yp - q.Yp;

        var beta = (q.Yp - p.Yp - (q.Xp - p.Xp) * vy / vx)
            / (ux * vy / vx - uy);
        
        var alfa = (beta * ux + q.Xp - p.Xp) / vx;

        return (alfa, beta) is (>=0f and <=1f, >=0f and <=1f);
    }

    /// <summary>
    /// Test if a line with the points (qx, qy) nad (qx, infity)
    /// intersects with another line.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool RayIntersect(
        ref PlanarVertex p, ref PlanarVertex pf,
        float qx, float qy
    )
    {
        // See Intersect function for more details.
        // qf = (qx, infinity)
        // uy = infinity
        // beta = (q.Yp - p.Yp - (q.Xp - p.Xp) * vy / vx) / (ux * vy / vx - uy)
        // beta = 0
        // alfa = (beta * ux + q.Xp - p.Xp) / vx;
        // alfa = (q.Xp - p.Xp) / vx
        
        var almost_infty = 1e30f;
        var vx = pf.Xp - p.Xp;
        var vy = pf.Yp - p.Yp;
        var ux = qx - qx;
        var uy = almost_infty - qy;

        var beta = (qy - p.Yp - (qx - p.Xp) * vy / vx)
            / (ux * vy / vx - uy);
        
        var alfa = (beta * ux + qx - p.Xp) / vx;

        return (alfa, beta) is (>=0f and <=1f, >=0f and <=1f);

    }
}