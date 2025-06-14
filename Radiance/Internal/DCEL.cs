/* Author:  Leonardo Trevisan Silio
 * Date:    12/06/2025
 */
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Radiance.Internal;

/// <summary>
/// Represents a Double Connected Edge List.
/// </summary>
public class DCEL
{
    const float almost_infty = 1e6f;
    int nextEdgeId = 0;
    int nextFaceId = 0;
    readonly Vertex[] Source;
    public readonly int Length;
    public readonly List<HalfEdge> Edges = [];
    public readonly Dictionary<int, List<HalfEdge>> FromEdgeMap = [];
    public readonly Dictionary<int, List<HalfEdge>> ToEdgeMap = [];
    public readonly Dictionary<int, List<int>> Faces = [];
    public readonly Dictionary<int, List<HalfEdge>> FacesEdges = [];

    public DCEL(Vertex[] source, int[] points)
    {
        Source = source;
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

    public DCEL(float[] points)
    {
        points = FixClockwise(points);
        var vertexes = new Vertex[points.Length / 3];
        for (int j = 0, k = 0; j < points.Length; j += 3, k++)
            vertexes[k] = new Vertex(k, points[j], points[j + 1], points[j + 2]);

        Source = vertexes;
        Length = Source.Length;

        int face = CreateFace();
        List<int> faceVertexes = Faces[face];
        List<HalfEdge> faceEdges = FacesEdges[face];

        for (int j = 0; j < Source.Length; j++)
            faceVertexes.Add(j);

        HalfEdge fst, prv;
        fst = prv = CreateEdge(0, 1, face);

        int i = 1;
        while (i < Source.Length - 1)
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
    /// Create a SweepLine from this DCEL.
    /// </summary>
    public SweepLine CreateSweepLine()
        => new (Source, new int[Source.Length]);
    
    /// <summary>
    /// Receiving 2 ids for vertex return if them are connected.
    /// </summary>
    public bool IsConnected(int v, int u)
        => FromEdgeMap[v].Any(e => e.To == u) 
        || FromEdgeMap[u].Any(e => e.To == v);

    /// <summary>
    /// Add a Edge between two vertex.
    /// </summary>
    public bool Connect(int v, int u)
    {
        System.Console.WriteLine($"Connect({v}, {u})");
        if (v == u)
            return false;
        
        var faceId = GetSharedFace(v, u);
        if (faceId is null)
            return false;
        
        if (IsConnected(v, u))
            return false;
        
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
    public bool CanInternalConnect(int vid, int uid)
    {
        var v = GetVertex(vid);
        var u = GetVertex(uid);

        foreach (var edge in Edges)
        {
            if (edge.From == vid || edge.To == vid || edge.From == uid || edge.To == uid)
                continue;

            var v2 = GetVertex(edge.From);
            var u2 = GetVertex(edge.To);

            if (Intersect(v, u, v2, u2))
                return false;
        }
        
        return IsInside((v.X + u.X) / 2, (v.Y + u.Y) / 2);
    }

    /// <summary>
    /// Get two hash set of the left and right chain over a sweep line.
    /// </summary>
    public (HashSet<int> left, HashSet<int> right) GetChains(SweepLine sweepLine)
    {
        var top = sweepLine[0].Id;
        var bottom = sweepLine[^1].Id;
        var current = top;

        HashSet<int> leftChain = [ top, bottom ];
        HashSet<int> rightChain = [ ];

        while (current != bottom)
        {
            leftChain.Add(current);
            current = FromEdgeMap[current][0].To;
        }
        
        current = FromEdgeMap[current][0].To;
        while (current != top)
        {
            rightChain.Add(current);
            current = FromEdgeMap[current][0].To;
        }

        return (rightChain, leftChain);
    }

    /// <summary>
    /// Discover the type of the vertex with specific id.
    /// </summary>
    public VertexType DiscoverType(int v)
    {
        var edges = FromEdgeMap[v];
        var edge = edges[0];
        var self = GetVertex(v);
        var e1 = GetVertex(edge.To);
        var e2 = GetVertex(edge.Previous!.From);
        
        if (over(self, e1) && over(self, e2))
            return Left(e1, self, e2) > 0 ?
                VertexType.Split : VertexType.Start;
        
        if (over(e1, self) && over(e2, self))
            return Left(e1, self, e2) > 0 ?
                VertexType.Merge : VertexType.End;

        return VertexType.Regular;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool over(Vertex p, Vertex q)
            => p.Y > q.Y || (p.Y == q.Y && p.X < q.X);
    }

    /// <summary>
    /// Find the left edge from a vertex. If are two left
    /// edges the algorithm choose the least y-axis. 
    /// </summary>
    public int FindLeftEdge(int vertexId)
    {
        var vert = GetVertex(vertexId);
        var y = vert.Y;
        var x = vert.X;

        int selected = -1;
        float bestX = float.MaxValue;

        foreach (var edge in Edges)
        {
            if (edge.From == vertexId)
                continue;
            
            if (edge.To == vertexId)
                continue;

            var v = GetVertex(edge.To);
            var x1 = v.X;
            var y1 = v.Y;

            var u = GetVertex(edge.From);
            var x2 = u.X;
            var y2 = u.Y;

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
    public DCEL ApplyFilter(int[] points)
        => new (Source, points);

    /// <summary>
    /// Get the face shader by two vertex
    /// with id 'v' and 'u'.
    /// </summary>
    public int? GetSharedFace(int vid, int uid)
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
    public int[] RemoveSubPolygon()
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
    public bool LiesOnRight(int vid)
    {
        var vert = GetVertex(vid);
        return IsInside(vert.X + 1 / almost_infty, vert.Y);
    }

    /// <summary>
    /// Get a array of points.
    /// </summary>
    public float[] ToArray()
    {
        List<float> values = [];
        foreach (var face in Faces)
        {
            foreach (var vertexId in face.Value)
            {
                var vertex = GetVertex(vertexId);
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
    public Vertex GetVertex(int id)
        => Source[id];

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
        var fromEdges = GetFromEdgeList(from);
        var toEdges = GetToEdgeList(to);
        
        fromEdges.Add(edge);
        toEdges.Add(edge);
        Edges.Add(edge);

        var faceEdges = GetFaceEdgeList(face);
        faceEdges.Add(edge);

        return edge;
    }

    /// <summary>
    /// Get, and init if needed, edges connect
    /// to a vertex with specific id.
    /// </summary>
    List<HalfEdge> GetFromEdgeList(int id)
    {
        if (FromEdgeMap.TryGetValue(id, out var edges))
            return edges;
        
        edges = [];
        FromEdgeMap.Add(id, edges);
        return edges;
    }

    /// <summary>
    /// Get, and init if needed, edges connect
    /// to a vertex with specific id.
    /// </summary>
    List<HalfEdge> GetToEdgeList(int id)
    {
        if (ToEdgeMap.TryGetValue(id, out var edges))
            return edges;
        
        edges = [];
        ToEdgeMap.Add(id, edges);
        return edges;
    }

    /// <summary>
    /// Get, and init if needed, edges in
    /// a specific face.
    /// </summary>
    List<HalfEdge> GetFaceEdgeList(int id)
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
    public float Left(int pid, int qId, int rId)
    {
        var p = GetVertex(pid);
        var q = GetVertex(qId);
        var r = GetVertex(rId);
        return Left(p, q, r);
    }

    /// <summary>
    /// Compute area from this a collection of points. Returns
    /// negative when points are anti-clockwise. 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float Area(float[] points)
    {
        var area = 0f;
        for (int i = 0; i < points.Length - 3; i += 3)
        {
            var x1 = points[i];
            var x2 = points[i + 3];
            var y1 = points[i + 1];
            var y2 = points[i + 1 + 3];
            area += x1 * y2 - x2 * y1;
        }
        area += points[^3] * points[1] - points[0] * points[^2];
        return area / 2;
    }

    /// <summary>
    /// Reverse the (x, y, z) pairs.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float[] Reverse(float[] points)
    {
        float[] reversed = new float[points.Length];
        for (int i = 0; i < points.Length; i += 3)
        {
            reversed[i] = points[^(i + 3)];
            reversed[i + 1] = points[^(i + 2)];
            reversed[i + 2] = points[^(i + 1)];
        }
        return reversed;
    }

    /// <summary>
    /// Fix clockwise to pairs (x, y, z).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float[] FixClockwise(float[] points)
    {
        var area = Area(points);
        if (area > 0)
            return points;
        return Reverse(points);
    }

    /// <summary>
    /// The left operation. https://en.wikipedia.org/wiki/Left_and_right_(algebra)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float Left(Vertex p, Vertex q, Vertex r)
        => Left(p.X, p.Y, q.X, q.Y, r.X, r.Y);
    
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
        Vertex p, Vertex pf,
        Vertex q, Vertex qf
    )
    {
        // alfa * vx + px = beta * ux + qx
        // alfa * vy + py = beta * uy + qy
        // alfa = (beta * ux + qx - px) / vx
        // vy * (beta * ux + qx - px) / vx + py = beta * uy + qy
        // beta * ux * vy / vx - beta * uy = qy - py - (qx - px) * vy / vx
        // beta * (ux * vy / vx - uy) = qy - py - (qx - px) * vy / vx
        
        var vx = pf.X - p.X;
        var vy = pf.Y - p.Y;
        var ux = qf.X - q.X;
        var uy = qf.Y - q.Y;

        var beta = (q.Y - p.Y - (q.X - p.X) * vy / vx)
            / (ux * vy / vx - uy);
        
        var alfa = (beta * ux + q.X - p.X) / vx;

        return (alfa, beta) is (>0f and <1f, >0f and <1f);
    }

    /// <summary>
    /// Test if a line with the points (qx, qy) nad (qx, infity)
    /// intersects with another line.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool RayIntersect(
        Vertex p, Vertex pf,
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
        
        var vx = pf.X - p.X;
        var vy = pf.Y - p.Y;
        var ux = qx - qx;
        var uy = almost_infty - qy;

        var beta = (qy - p.Y - (qx - p.X) * vy / vx)
            / (ux * vy / vx - uy);
        
        var alfa = (beta * ux + qx - p.X) / vx;

        return (alfa, beta) is (>0f and <1f, >0f and <1f);

    }

    bool IsInside(float px, float py)
    {
        int count = 0;

        foreach (var edge in Edges)
        {
            var v2 = GetVertex(edge.From);
            var u2 = GetVertex(edge.To);

            if (RayIntersect(v2, u2, px, py))
                count++;
        }

        return count % 2 == 1;
    }
}