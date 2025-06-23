/* Author:  Leonardo Trevisan Silio
 * Date:    23/06/2025
 */
using System;
using System.Linq;
using System.Text;
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
    readonly List<Vertex> Source;
    readonly Dictionary<int, VertexType> VertexesTypes = [];
    readonly HashSet<int> HoleSet = [];
    public int Length => Source.Count;
    public readonly List<HalfEdge> Edges = [];
    public readonly Dictionary<int, Vertex> Vertexes = [];
    public readonly Dictionary<int, List<HalfEdge>> FromEdgeMap = [];
    public readonly Dictionary<int, List<HalfEdge>> ToEdgeMap = [];
    public readonly Dictionary<int, List<int>> Faces = [];
    public readonly Dictionary<int, List<HalfEdge>> FacesEdges = [];
    public IEnumerable<int> FaceIds => Faces.Keys;

    public DCEL(List<Vertex> contour, List<List<Vertex>> holes)
    {
        FixClockwise(contour);
        foreach (var hole in holes)
            FixHoleClockwise(hole);
        
        Source = [ ..contour ];
        foreach (var hole in holes)
        {
            Source.AddRange(hole);
            foreach (var point in hole)
                HoleSet.Add(point.Id);
        }
            
        foreach (var vertex in Source)
            Vertexes.Add(vertex.Id, vertex);
        
        var face = CreateFace();
        List<int> faceVertexes = Faces[face];

        foreach (var point in contour)
            faceVertexes.Add(point.Id);
        
        foreach (var hole in holes)
            foreach (var point in hole)
                faceVertexes.Add(point.Id);

        HalfEdge fst, prv;
        fst = prv = CreateEdge(
            contour[0].Id,
            contour[1].Id,
            face
        );

        int i = 1;
        while (i < contour.Count - 1)
        {
            var crr = CreateEdge(
                contour[i].Id,
                contour[i + 1].Id,
                face
            );
            crr.SetPrevious(prv);

            prv = crr;
            i++;
        }

        var lst = CreateEdge(
            contour[i].Id, 
            contour[0].Id,
            face
        );
        lst.SetPrevious(prv);
        lst.SetNext(fst);

        foreach (var hole in holes)
        {
            fst = prv = CreateEdge(
                hole[0].Id,
                hole[1].Id,
                face
            );

            i = 1;
            while (i < hole.Count - 1)
            {
                var crr = CreateEdge(
                    hole[i].Id,
                    hole[i + 1].Id,
                    face
                );
                crr.SetPrevious(prv);

                prv = crr;
                i++;
            }

            lst = CreateEdge(
                hole[i].Id, 
                hole[0].Id,
                face
            );
            lst.SetPrevious(prv);
            lst.SetNext(fst);
        }
    }

    public DCEL(Vertex[] source)
    {
        Source = [ ..source ];
        foreach (var vertex in source)
            Vertexes.Add(vertex.Id, vertex);

        int face = CreateFace();
        List<int> faceVertexes = Faces[face];

        for (int j = 0; j < source.Length; j++)
            faceVertexes.Add(source[j].Id);

        HalfEdge fst, prv;
        fst = prv = CreateEdge(
            source[0].Id,
            source[1].Id,
            face
        );

        int i = 1;
        while (i < source.Length - 1)
        {
            var crr = CreateEdge(
                source[i].Id,
                source[i + 1].Id,
                face
            );
            crr.SetPrevious(prv);

            prv = crr;
            i++;
        }

        var lst = CreateEdge(
            source[i].Id, 
            source[0].Id,
            face
        );
        lst.SetPrevious(prv);
        lst.SetNext(fst);
    }

    public DCEL(float[] points)
    {
        points = FixClockwise(points);
        Source = [];
        for (int j = 0, k = 0; j < points.Length; j += 3, k++)
        {
            var vert = new Vertex(k, points[j], points[j + 1], points[j + 2]);
            Source.Add(vert);
            Vertexes.Add(k, vert);
        }

        int face = CreateFace();
        List<int> faceVertexes = Faces[face];

        for (int j = 0; j < Source.Count; j++)
            faceVertexes.Add(j);

        HalfEdge fst, prv;
        fst = prv = CreateEdge(0, 1, face);

        int i = 1;
        while (i < Source.Count - 1)
        {
            var crr = CreateEdge(
                i,
                i + 1,
                face
            );
            crr.SetPrevious(prv);

            prv = crr;
            i++;
        }

        var lst = CreateEdge(
            i, 
            0,
            face
        );
        lst.SetPrevious(prv);
        lst.SetNext(fst);
    }

    /// <summary>
    /// Create a SweepLine from this DCEL.
    /// </summary>
    public SweepLine CreateSweepLine()
        => new (Source);
    
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
        if (v == u)
            return false;
        
        if (IsConnected(v, u))
            return false;
        
        // Maybe has some bugs when a point conects with
        // many lines.

        var e1 = CreateEdge(v, u, 0);
        var e2 = CreateEdge(u, v, 0);

        var nextv = e1;
        var anglev = AngleTo(GetVertex(u), GetVertex(v));
        var bestDiff = float.PositiveInfinity;
        foreach (var e in GetFromEdgeList(v))
        {
            if (e == e1)
                continue;
            
            var angle = AngleTo(GetVertex(e.To), GetVertex(e.From));
            if (angle < anglev)
                angle += MathF.Tau;
            var diff = anglev - angle;
            if (anglev - angle < bestDiff)
            {
                bestDiff = diff;
                nextv = e;
            }
        }
        var prevv = nextv.Previous!;

        var nextu = e2;
        var angleu = AngleTo(GetVertex(v), GetVertex(u));
        bestDiff = float.PositiveInfinity;
        foreach (var e in GetFromEdgeList(u))
        {
            if (e == e2)
                continue;
            
            var angle = AngleTo(GetVertex(e.To), GetVertex(e.From));
            if (angle < angleu)
                angle += MathF.Tau;
            var diff = angleu - angle;
            if (angleu - angle < bestDiff)
            {
                bestDiff = diff;
                nextu = e;
            }
        }
        var prevu = nextu.Previous!;

        e2.SetNext(nextv);
        e2.SetPrevious(prevu);

        e1.SetNext(nextu);
        e1.SetPrevious(prevv);

        return true;

        float AngleTo(Vertex toVert, Vertex fromVert)
        {
            var dx = toVert.X - fromVert.X;
            var dy = toVert.Y - fromVert.Y;
            return MathF.Atan2(dy, dx);
        }
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
    /// Get the Vertex type of a vertex with specific id.
    /// </summary>
    public VertexType GetVertexType(int vertexId)
    {
        if (VertexesTypes.TryGetValue(vertexId, out var type))
            return type;
        
        type = DiscoverType(vertexId);
        VertexesTypes[vertexId] = type;
        return type;
    }

    /// <summary>
    /// Get if the polygon is monotone.
    /// </summary>
    public bool IsMonotone 
    {
        get
        {
            foreach (var vertex in Source)
            {
                var type = GetVertexType(vertex.Id);
                if (type == VertexType.Merge)
                    return false;
                
                if (type == VertexType.Split)
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Find the left edge from a vertex. If are two left
    /// edges the algorithm choose the least y-axis. 
    /// </summary>
    public int FindLeftEdge(int vertexId)
    {
        var vert = GetVertex(vertexId);
        var x = vert.X;

        int selected = -1;
        float bestX = float.MinValue;

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

            var between = v > vert && vert > u || u > vert && vert > v;
            if (!between)
                continue;

            var minX = float.Min(x1, x2);
            if (minX > x)
                continue;

            var maxX = float.Max(x1, x2);
            if (maxX < bestX)
                continue;
            
            if (Left(v.Id, vertexId, u.Id) > 0)
                continue;

            bestX = maxX;
            selected = edge.Id;
        }
        
        return selected;
    }

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
    /// Get a subdcel from a face.
    /// </summary>
    public IEnumerable<DCEL> GetSubDCELs()
    {
        var queue = new Queue<int>(Source.Select(v => v.Id));
        var set = new HashSet<int>();

        while (queue.Count > 0)
        {
            var vert = queue.Dequeue();
            var edges = FromEdgeMap[vert];

            foreach (var edge in edges)
            {
                if (set.Contains(edge.Id))
                    continue;

                var fst = edge;
                var crr = fst;
                var end = edge.Previous;
                List<Vertex> subverts = [ GetVertex(fst.From) ];
                while (crr != end)
                {
                    set.Add(crr.Id);
                    subverts.Add(GetVertex(crr.To));
                    crr = crr.Next!;
                }
                set.Add(end.Id);
                var subDcel = new DCEL([ ..subverts ]);
                yield return subDcel;
            }
        }
    }

    /// <summary>
    /// Returns true if the polygon lies to the right of vi.
    /// </summary>
    public bool LiesOnRight(int vid)
    {
        var prev = GetVertex(ToEdgeMap[vid][0].From);
        var curr = GetVertex(vid);
        var next = GetVertex(FromEdgeMap[vid][0].To);

        return prev > curr && curr > next;
    }

    /// <summary>
    /// Get a array of points.
    /// </summary>
    public float[] ToArray()
    {
        List<float> values = [];
        var queue = new Queue<int>(Source.Select(v => v.Id));
        var set = new HashSet<int>();

        while (queue.Count > 0)
        {
            var vert = queue.Dequeue();
            var edges = FromEdgeMap[vert];

            foreach (var edge in edges)
            {
                if (set.Contains(edge.Id))
                    continue;

                var fst = edge;
                var crr = fst;
                var end = edge.Previous;
                List<Vertex> subverts = [ GetVertex(fst.From) ];
                while (crr != end)
                {
                    set.Add(crr.Id);
                    subverts.Add(GetVertex(crr.To));
                    crr = crr.Next!;
                }
                set.Add(end.Id);
                
                foreach (var point in subverts)
                {
                    values.Add(point.X);
                    values.Add(point.Y);
                    values.Add(point.Z);
                }
            }
        }
        return [ ..values ];
    }
    
    /// <summary>
    /// Get a Planar Vertex by id.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vertex GetVertex(int id)
        => Vertexes[id];

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var pt in Source)
            sb.AppendLine($$"""P_{{{pt.Id}}} = {{pt}}""");
        
        return sb.ToString();
    }

    /// <summary>
    /// Discover the type of the vertex with specific id.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    VertexType DiscoverType(int vertexId)
    {
        var edges = FromEdgeMap[vertexId];
        var edge = edges[0];
        var self = GetVertex(vertexId);
        var e1 = GetVertex(edge.To);
        var e2 = GetVertex(edge.Previous!.From);

        if (HoleSet.Contains(vertexId))
        {
            if (self > e1 && self > e2)
                return Left(e1, self, e2) < 0 ?
                    VertexType.Split : VertexType.Regular;
            
            if (e1 > self && e2 > self)
                return Left(e1, self, e2) < 0 ?
                    VertexType.Merge : VertexType.Regular;
        }
        else
        {
            if (self > e1 && self > e2)
                return Left(e1, self, e2) < 0 ?
                    VertexType.Split : VertexType.Start;
            
            if (e1 > self && e2 > self)
                return Left(e1, self, e2) < 0 ?
                    VertexType.Merge : VertexType.End;
        }

        return VertexType.Regular;
    }

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
    /// Compute area from this a collection of points. Returns
    /// negative when points are anti-clockwise. 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float Area(List<Vertex> points)
    {
        var area = 0f;
        for (int i = 0; i < points.Count - 1; i++)
        {
            var x1 = points[i].X;
            var x2 = points[i + 1].X;
            var y1 = points[i + 1].Y;
            var y2 = points[i + 1].Y;
            area += x1 * y2 - x2 * y1;
        }
        area += points[^1].X * points[0].Y - points[0].X * points[^1].Y;
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
    /// Fix clockwise to pairs (x, y, z).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool FixClockwise(List<Vertex> points)
    {
        var area = Area(points);
        if (area > 0)
            return false;
        points.Reverse();
        return true;
    }

    /// <summary>
    /// Fix clockwise to pairs (x, y, z).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool FixHoleClockwise(List<Vertex> points)
    {
        var area = Area(points);
        if (area < 0)
            return false;
        points.Reverse();
        return true;
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