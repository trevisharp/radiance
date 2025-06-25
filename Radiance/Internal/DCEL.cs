/* Author:  Leonardo Trevisan Silio
 * Date:    25/06/2025
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
    readonly List<Vertex> Source;
    readonly Dictionary<Vertex, VertexType> VertexesTypes = [];
    readonly HashSet<Vertex> HoleSet = [];
    public int Length => Source.Count;
    public readonly List<HalfEdge> Edges = [];
    public readonly Dictionary<Vertex, List<HalfEdge>> FromEdgeMap = [];
    public readonly Dictionary<Vertex, List<HalfEdge>> ToEdgeMap = [];

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
                HoleSet.Add(point);
        }

        HalfEdge fst, prv;
        fst = prv = CreateEdge(contour[0], contour[1]);

        int i = 1;
        while (i < contour.Count - 1)
        {
            var crr = CreateEdge(contour[i], contour[i + 1]);
            crr.SetPrevious(prv);

            prv = crr;
            i++;
        }

        var lst = CreateEdge(contour[i], contour[0]);
        lst.SetPrevious(prv);
        lst.SetNext(fst);

        foreach (var hole in holes)
        {
            fst = prv = CreateEdge(hole[0], hole[1]);

            i = 1;
            while (i < hole.Count - 1)
            {
                var crr = CreateEdge(hole[i], hole[i + 1]);
                crr.SetPrevious(prv);

                prv = crr;
                i++;
            }

            lst = CreateEdge(hole[i], hole[0]);
            lst.SetPrevious(prv);
            lst.SetNext(fst);
        }
    }

    public DCEL(List<Vertex> source)
    {
        Source = source;
        FixClockwise(Source);

        HalfEdge fst, prv;
        fst = prv = CreateEdge(source[0], source[1]);

        int i = 1;
        while (i < source.Count - 1)
        {
            var crr = CreateEdge(source[i], source[i + 1]);
            crr.SetPrevious(prv);

            prv = crr;
            i++;
        }

        var lst = CreateEdge(source[i], source[0]);
        lst.SetPrevious(prv);
        lst.SetNext(fst);
    }

    public DCEL(float[] points) : this(
        points
            .Chunk(3)
            .Select((arr) => new Vertex(arr[0], arr[1], arr[2]))
            .ToList()
    ) { }

    /// <summary>
    /// Create a SweepLine from this DCEL.
    /// </summary>
    public SweepLine CreateSweepLine()
        => new (Source);
    
    /// <summary>
    /// Receiving 2 ids for vertex return if them are connected.
    /// </summary>
    public bool IsConnected(Vertex v, Vertex u)
        => FromEdgeMap[v].Any(e => e.To == u) 
        || FromEdgeMap[u].Any(e => e.To == v);

    /// <summary>
    /// Add a Edge between two vertex.
    /// </summary>
    public bool Connect(Vertex v, Vertex u)
    {
        if (v == u)
            return false;
        
        if (IsConnected(v, u))
            return false;
        
        // Maybe has some bugs when a point conects with
        // many lines.

        var e1 = CreateEdge(v, u);
        var e2 = CreateEdge(u, v);

        var nextv = e1;
        var anglev = AngleTo(u, v);
        var bestDiff = float.PositiveInfinity;
        foreach (var e in GetFromEdgeList(v))
        {
            if (e == e1)
                continue;
            
            var angle = AngleTo(e.To, e.From);
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
        var angleu = AngleTo(v, u);
        bestDiff = float.PositiveInfinity;
        foreach (var e in GetFromEdgeList(u))
        {
            if (e == e2)
                continue;
            
            var angle = AngleTo(e.To, e.From);
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
    bool CanInternalConnect(Vertex v, Vertex u)
    {
        foreach (var edge in Edges)
        {
            if (edge.From == v || edge.To == v || edge.From == u || edge.To == u)
                continue;

            if (Intersect(v, u, edge.From, edge.To))
                return false;
        }
        
        return IsInside((v.X + u.X) / 2, (v.Y + u.Y) / 2);
    }

    /// <summary>
    /// Get two hash set of the left and right chain over a sweep line.
    /// </summary>
    (HashSet<Vertex> left, HashSet<Vertex> right) GetChains(SweepLine sweepLine)
    {
        var top = sweepLine[0];
        var bottom = sweepLine[^1];
        var current = top;

        HashSet<Vertex> leftChain = [ top, bottom ];
        HashSet<Vertex> rightChain = [ ];

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
    VertexType GetVertexType(Vertex vertex)
    {
        if (VertexesTypes.TryGetValue(vertex, out var type))
            return type;
        
        type = DiscoverType(vertex);
        VertexesTypes[vertex] = type;
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
                var type = GetVertexType(vertex);
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
    HalfEdge FindLeftEdge(Vertex vertex)
    {
        var x = vertex.X;

        HalfEdge? selected = null;
        float bestX = float.MinValue;

        foreach (var edge in Edges)
        {
            if (edge.From == vertex)
                continue;
            
            if (edge.To == vertex)
                continue;

            var v = edge.To;
            var x1 = v.X;
            var y1 = v.Y;

            var u = edge.From;
            var x2 = u.X;
            var y2 = u.Y;

            var between = v > vertex && vertex > u || u > vertex && vertex > v;
            if (!between)
                continue;

            var minX = float.Min(x1, x2);
            if (minX > x)
                continue;

            var maxX = float.Max(x1, x2);
            if (maxX < bestX)
                continue;
            
            if (Left(v, vertex, u) > 0)
                continue;

            bestX = maxX;
            selected = edge;
        }

        if (selected is null)
            throw new NullReferenceException($"Vertex has no Left Edge.");
        
        return selected;
    }

    /// <summary>
    /// Get a subdcel from a face.
    /// </summary>
    public IEnumerable<DCEL> GetSubDCELs()
    {
        var queue = new Queue<Vertex>(Source);
        var set = new HashSet<HalfEdge>();

        while (queue.Count > 0)
        {
            var vert = queue.Dequeue();
            var edges = FromEdgeMap[vert];

            foreach (var edge in edges)
            {
                if (set.Contains(edge))
                    continue;

                var fst = edge;
                var crr = fst;
                var end = edge.Previous;
                List<Vertex> subverts = [ fst.From ];
                while (crr != end)
                {
                    set.Add(crr);
                    subverts.Add(crr.To);
                    crr = crr.Next!;
                }
                set.Add(end);
                var subDcel = new DCEL(subverts);
                yield return subDcel;
            }
        }
    }

    /// <summary>
    /// Returns true if the polygon lies to the right of vi.
    /// </summary>
    bool LiesOnRight(Vertex vertex)
    {
        var prev = ToEdgeMap[vertex][0].From;
        var next = FromEdgeMap[vertex][0].To;

        return prev > vertex && vertex > next;
    }

    /// <summary>
    /// Get a array of points.
    /// </summary>
    public float[] ToArray()
    {
        List<float> values = [];
        var queue = new Queue<Vertex>(Source);
        var set = new HashSet<HalfEdge>();

        while (queue.Count > 0)
        {
            var vert = queue.Dequeue();
            var edges = FromEdgeMap[vert];

            foreach (var edge in edges)
            {
                if (set.Contains(edge))
                    continue;

                var fst = edge;
                var crr = fst;
                var end = edge.Previous;
                List<Vertex> subverts = [ fst.From ];
                while (crr != end)
                {
                    set.Add(crr);
                    subverts.Add(crr.To);
                    crr = crr.Next!;
                }
                set.Add(end);
                
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
    
    public override string ToString()
    {
        var sb = new StringBuilder();
        int i = 0;
        foreach (var pt in Source)
            sb.AppendLine($$"""P_{{{i++}}} = {{pt}}""");
        
        return sb.ToString();
    }

    /// <summary>
    /// Get a triangulation of a polygon with points in a
    /// clockwise order.
    /// </summary>
    public float[] GetTriangules()
    {
        if (MonotoneDivision(this))
            return NonMonotonePlaneTriangularization(this);
        
        return MonotonePlaneTriangulation(this);
    }

    /// <summary>
    /// Divide a polygon on many monotone polygons.
    /// Return true if some polygon has created.
    /// </summary>
    static bool MonotoneDivision(DCEL dcel)
    {
        if (dcel.IsMonotone)
            return false;

        var sweepLine = dcel.CreateSweepLine();
        Dictionary<HalfEdge, Vertex> helper = [];
        
        for (int i = 0; i < sweepLine.Length; i++)
        {
            var v = sweepLine[i];
            
            var type = dcel.GetVertexType(v);
            var ei = dcel.FromEdgeMap[v][0];
            var eprev = dcel.ToEdgeMap[v][0];
            
            switch (type)
            {
                case VertexType.Start:

                    helper[ei] = v;

                    break;
                    
                case VertexType.End:
                    
                    if (dcel.GetVertexType(helper[eprev]) == VertexType.Merge)
                    {
                        dcel.Connect(v, helper[eprev]);
                    }

                    break;

                case VertexType.Split:

                    var ej1 = dcel.FindLeftEdge(v);
                    dcel.Connect(helper[ej1], v);
                    helper[ej1] = v;
                    helper[ei] = v;

                    break;

                case VertexType.Merge:

                    if (dcel.GetVertexType(helper[eprev]) == VertexType.Merge)
                    {
                        dcel.Connect(v, helper[eprev]);
                    }
                    
                    var ej2 = dcel.FindLeftEdge(v);
                    if (dcel.GetVertexType(helper[ej2]) == VertexType.Merge)
                    {
                        dcel.Connect(helper[ej2], v);
                    }
                    
                    helper[ej2] = v;

                    break;

                case VertexType.Regular:

                    if (dcel.LiesOnRight(v))
                    {
                        if (dcel.GetVertexType(helper[eprev]) == VertexType.Merge)
                        {
                            dcel.Connect(v, helper[eprev]);
                        }

                        helper[ei] = v;
                    }
                    else
                    {
                        var ej3 = dcel.FindLeftEdge(v);
                        if (dcel.GetVertexType(helper[ej3]) == VertexType.Merge)
                        {
                            dcel.Connect(helper[ej3], v);
                        }
                        helper[ej3] = v;
                    }

                    break;
            }
        }

        return true;
    }

    /// <summary>
    /// Get a nonmonotone DCEL divide inot monotone polygons and returns
    /// the triangularization.
    /// </summary>
    static float[] NonMonotonePlaneTriangularization(DCEL dcel)
    {
        var index = 0;
        var triangules = new List<float>();

        float[] data;
        var subdcels = dcel.GetSubDCELs().ToArray();
        foreach (var subDcel in subdcels)
        {
            if (subDcel.Length < 4)
            {
                data = subDcel.ToArray();
                triangules.AddRange(data);
                index += data.Length;
                continue;
            }
            
            data = MonotonePlaneTriangulation(subDcel);
            triangules.AddRange(data);
            index += data.Length;
        }

        return [ ..triangules ];
    }

    /// <summary>
    /// Receveing a map of ordenation and data with format (x, y, z, ...),
    /// if the points represetns a monotone polygon, return the triangularization
    /// of then.
    /// </summary>
    static float[] MonotonePlaneTriangulation(DCEL dcel)
    {
        var sweepLine = dcel.CreateSweepLine();
        var (leftChain, rightChain) = dcel.GetChains(sweepLine);

        var stack = new Stack<Vertex>();
        stack.Push(sweepLine[0]);
        stack.Push(sweepLine[1]);

        for (int j = 2; j < dcel.Length - 1; j++)
        {
            var vtop = stack.Peek();
            var vj = sweepLine[j];

            var topInChainA = leftChain.Contains(vtop);
            var nextInChainA = leftChain.Contains(vj);
            var sameChain = topInChainA == nextInChainA;

            if (sameChain)
            {
                var popped = stack.Pop();
                while (stack.Count > 0)
                {
                    var next = stack.Peek();

                    if (!dcel.CanInternalConnect(next, vj))
                        break;

                    popped = stack.Pop();
                    dcel.Connect(vj, popped);
                }
                stack.Push(popped);
                stack.Push(vj);
            }
            else
            {
                var vj_1 = sweepLine[j - 1];
                while (stack.Count > 1)
                {
                    var vk = stack.Pop();
                    dcel.Connect(vj, vk);
                }
                stack.Pop();
                stack.Push(vj_1);
                stack.Push(vj);
            }
        }

        var vn = sweepLine[^1];
        stack.Pop();

        while (stack.Count > 1)
        {
            var vk = stack.Pop();
            dcel.Connect(vk, vn);
        }

        return dcel.ToArray();
    }

    /// <summary>
    /// Discover the type of the vertex with specific id.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    VertexType DiscoverType(Vertex vertex)
    {
        var edges = FromEdgeMap[vertex];
        var edge = edges[0];
        var self = vertex;
        var e1 = edge.To;
        var e2 = edge.Previous!.From;

        if (HoleSet.Contains(vertex))
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
    /// Create a new Edge between 'from' and 'to'
    /// on specific face. Do not create new face
    /// and do not keep face consistency.
    /// </summary>
    HalfEdge CreateEdge(Vertex from, Vertex to)
    {
        var edge = new HalfEdge(from, to);
        var fromEdges = GetFromEdgeList(from);
        var toEdges = GetToEdgeList(to);
        
        fromEdges.Add(edge);
        toEdges.Add(edge);
        Edges.Add(edge);

        return edge;
    }

    /// <summary>
    /// Get, and init if needed, edges connect
    /// to a vertex with specific id.
    /// </summary>
    List<HalfEdge> GetFromEdgeList(Vertex vertex)
    {
        if (FromEdgeMap.TryGetValue(vertex, out var edges))
            return edges;
        
        edges = [];
        FromEdgeMap.Add(vertex, edges);
        return edges;
    }

    /// <summary>
    /// Get, and init if needed, edges connect
    /// to a vertex with specific id.
    /// </summary>
    List<HalfEdge> GetToEdgeList(Vertex vertex)
    {
        if (ToEdgeMap.TryGetValue(vertex, out var edges))
            return edges;
        
        edges = [];
        ToEdgeMap.Add(vertex, edges);
        return edges;
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
            if (RayIntersect(edge.From, edge.To, px, py))
                count++;
        }

        return count % 2 == 1;
    }
}