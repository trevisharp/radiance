/* Author:  Leonardo Trevisan Silio
 * Date:    30/12/2024
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Radiance.Internal;

/*
 * Future improvements:
 * -FindById affect performance. In a initial moment PlanarVertex.Id is equal
 *      to ther index on Vertexex prop, but for complex polygons on monotone
 *      division this assimetry is broke and FindById is needed.
 * -Other problem related with the first item is some code that use index
 *      equals Id and do not will broken with simple polygons but can
 *      broke in the future with complex polygons.
 */

/// <summary>
/// Represents a Double Connected Edge List.
/// </summary>
public ref struct DCEL
{
    int nextEdgeId = 0;
    int nextFaceId = 0;
    public readonly Span<PlanarVertex> Vertexes;
    public readonly Dictionary<int, List<HalfEdge>> Edges = [];
    public readonly Dictionary<int, List<int>> Faces = [];
    public readonly Dictionary<int, List<HalfEdge>> FacesEdges = [];

    public DCEL(Span<PlanarVertex> points)
    {
        Vertexes = points;

        int face = CreateFace();
        List<int> faceVertexes = Faces[face];
        List<HalfEdge> faceEdges = FacesEdges[face];

        for (int j = 0; j < points.Length; j++)
            faceVertexes.Add(Vertexes[j].Id);

        HalfEdge fst, prv;
        fst = prv = CreateEdge(
            Vertexes[0].Id,
            Vertexes[1].Id,
            face
        );

        int i = 1;
        while (i < points.Length - 1)
        {
            var crr = CreateEdge(
                Vertexes[i].Id,
                Vertexes[i + 1].Id,
                face
            );
            faceEdges.Add(crr);
            crr.SetPrevious(prv);

            prv = crr;
            i++;
        }

        var lst = CreateEdge(
            Vertexes[i].Id, 
            Vertexes[0].Id,
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
        => Edges[v].Any(e => e.To == u) 
        || Edges[u].Any(e => e.To == v);

    /// <summary>
    /// Add a Edge between two vertex.
    /// </summary>
    public void Connect(int v, int u)
    {
        System.Console.WriteLine($"connect {v} to {u}");
        if (v == u)
            return;
        
        var currFace = GetSharedFace(v, u);
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
        System.Console.WriteLine(
            string.Join(
                ", ", currEdges
            )
        );

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
        System.Console.WriteLine(
            string.Join(
                ", ", othrEdges
            )
        );
    }

    /// <summary>
    /// Discover the type of the vertex with specific id.
    /// </summary>
    public readonly VertexType DiscoverType(int v)
    {
        var edges = Edges[v];
        var edge = edges[0];
        ref var self = ref Vertexes[v];
        ref var e1 = ref Vertexes[edge.To];
        ref var e2 = ref Vertexes[edge.Previous!.From];
        
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
    /// Find the left edge from a vertex.
    /// </summary>
    public readonly int FindLeftEdge(int v)
    {
        var vert = Vertexes[v];
        var level = vert.Yp;
        var xpos = vert.Xp;
        bool? lastRelation = null;

        var crr = v;
        while (true)
        {
            var next = Edges[crr][0].To;
            var newLevel = Vertexes[next].Yp;
            var newRelation = newLevel < level;

            crr = next;

            lastRelation ??= newRelation;
            if (lastRelation == newRelation)
                continue;
            
            lastRelation = newRelation;
            if (Vertexes[next].Xp > xpos)
                continue;
            
            return crr;
        }
    }

    /// <summary>
    /// Get the face shader by two vertex
    /// with id 'v' and 'u'.
    /// </summary>
    readonly int GetSharedFace(int v, int u)
    {
        var (x, y) = GetMidPoint(ref FindById(v), ref FindById(u));

        for (int i = 0; i < Faces.Count; i++)
        {
            if (IsInFace(x, y, i))
                return i;
        }

        throw new Exception("point out of face");
    }

    /// <summary>
    /// Get if a point is in a face.
    /// </summary>
    readonly bool IsInFace(float x, float y, int faceId)
    {
        var face = Faces[faceId];

        for (int i = 0; i < face.Count; i++)
        {
            int j = (i + 1) % face.Count;

            var vertexAId = face[i];
            var vertexBId = face[j];

            ref var vertexA = ref FindById(vertexAId);
            ref var vertexB = ref FindById(vertexBId);

            if (Left(vertexA.Xp, vertexA.Yp, vertexB.Xp, vertexB.Yp, x, y) < 0)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Remove a random subpolygon and return a new DCEL.
    /// </summary>
    public readonly DCEL RemoveSubPolygon()
    {
        var face = Faces.Keys.Last();
        var points = Faces[face];
        Faces.Remove(face);

        var edges = FacesEdges[face];
        FacesEdges.Remove(face);
        var edge = edges[0];

        var vertexes = new PlanarVertex[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            vertexes[i] = Vertexes[edge.From];
            edge = edge.Next!;
        }

        return new DCEL(vertexes);
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
                ref var vertex = ref FindById(vertexId);
                values.Add(vertex.X);
                values.Add(vertex.Y);
                values.Add(vertex.Z);
            }
        }
        return [.. values];
    }

    /// <summary>
    /// Find a Planar Vertex by id.
    /// </summary>
    public readonly ref PlanarVertex FindById(int id)
    {
        for (int i = 0; i < Vertexes.Length; i++)
        {
            ref var vertex = ref Vertexes[i];
            if (vertex.Id == id)
                return ref vertex;
        }
        throw new Exception("Invalid vertex id");
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
        var edges = GetEdgeList(from);
        edges.Add(edge);

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
        if (Edges.TryGetValue(id, out var edges))
            return edges;
        
        edges = [];
        Edges.Add(id, edges);
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
        ref var p = ref FindById(pid);
        ref var q = ref FindById(qId);
        ref var r = ref FindById(rId);
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
    /// Get the mid point between two points.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static (float x, float y) GetMidPoint(ref PlanarVertex p, ref PlanarVertex q)
        => ((p.Xp + q.Xp) / 2, (p.Yp + q.Yp) / 2);
}