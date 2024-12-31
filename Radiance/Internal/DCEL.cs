/* Author:  Leonardo Trevisan Silio
 * Date:    29/12/2024
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Radiance.Internal;

/// <summary>
/// Represents a Double Connected Edge List.
/// </summary>
public ref struct DCEL
{
    int nextEdgeId = 0;
    int nextFaceId = 1;
    public readonly Span<PlanarVertex> Vertexs;
    public readonly Dictionary<int, List<HalfEdge>> Edges = [];
    public readonly Dictionary<int, List<int>> Faces = [];
    public readonly Dictionary<int, List<HalfEdge>> FacesEdges = [];

    public DCEL(Span<PlanarVertex> points)
    {
        Vertexs = points;

        HalfEdge fst, prv;
        fst = prv = CreateEdge(0, 1, 0);

        int i = 1;
        while (i < points.Length - 1)
        {
            var crr = CreateEdge(i, i + 1, 0);
            crr.SetPrevious(prv);

            prv = crr;
            i++;
        }

        var lst = CreateEdge(i, 0, 0);
        lst.SetPrevious(prv);
        lst.SetNext(fst);

        List<int> vertexes = [];
        Faces[0] = vertexes;
        for (int j = 0; j < points.Length; j++)
            vertexes.Add(j);
    }

    /// <summary>
    /// Receiving 2 ids for vertex return if them are connected.
    /// </summary>
    public readonly bool IsConnected(int v, int u)
        => Edges[v].Any(e => e.From == u) 
        || Edges[u].Any(e => e.From == v);

    /// <summary>
    /// Add a Edge between two vertex.
    /// </summary>
    public void Connect(int v, int u)
    {
        if (v == u)
            return;
        
        var faceA = GetSharedFace(v, u);
        var faceB = CreateFace();
        
        List<int> faceAPoints = [];
        List<int> faceBPoints = [];

        List<HalfEdge> faceEdgesA = [];
        List<HalfEdge> faceEdgesB = [];

        var sharedEdges = GetEdgeList(faceA);
        var fstEdge = sharedEdges[0];
        var crrEdge = fstEdge;
        
        do
        {
            var vertex = crrEdge.From;
            crrEdge.FaceId = faceA;
            faceAPoints.Add(vertex);
            faceEdgesA.Add(crrEdge);
            crrEdge = crrEdge.Next!;

            if (vertex == v)
            {
                faceAPoints.Add(u);
                (faceA, faceB) = (faceB, faceA);
                (faceAPoints, faceBPoints) = (faceBPoints, faceAPoints);
                (faceEdgesA, faceEdgesB) = (faceEdgesB, faceEdgesA);
            }

            if (vertex == u)
            {
                faceAPoints.Add(v);
                (faceA, faceB) = (faceB, faceA);
                (faceAPoints, faceBPoints) = (faceBPoints, faceAPoints);
                (faceEdgesA, faceEdgesB) = (faceEdgesB, faceEdgesA);
            }

        } while (crrEdge != fstEdge);

        Faces[faceA] = faceAPoints;
        Faces[faceB] = faceBPoints;
        FacesEdges[faceA] = faceEdgesA;
        FacesEdges[faceB] = faceEdgesB;

        var e1 = CreateEdge(v, u, faceA);
        faceEdgesA.Add(e1);
        foreach (var e in faceEdgesA)
        {
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

        var e2 = CreateEdge(u, v, faceB);
        faceEdgesB.Add(e2);
        foreach (var e in faceEdgesB)
        {
            if (e.To == v)
            {
                e.SetNext(e2);
                continue;
            }
            
            if (e.From == u)
            {
                e.SetPrevious(e2);
                continue;
            }
        }
    }

    /// <summary>
    /// Discover the type of the vertex with specific id.
    /// </summary>
    public readonly VertexType DiscoverType(int v)
    {
        var edges = Edges[v];
        ref var self = ref Vertexs[v];
        ref var e1 = ref Vertexs[edges[0].Id];
        ref var e2 = ref Vertexs[edges[1].Id];
        
        if (over(ref self, ref e1) && over(ref self, ref e2))
            return Left(ref e1, ref self, ref e2) < 0 ?
                VertexType.Split : VertexType.Start;
        
        if (over(ref e1, ref self) && over(ref e2, ref self))
            return Left(ref e1, ref self, ref e2) < 0 ?
                VertexType.Merge : VertexType.End;

        return VertexType.Regular;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool over(ref PlanarVertex p, ref PlanarVertex q)
            => p.Yp > q.Yp || (p.Yp == q.Yp && p.Xp > q.Xp);
    }

    /// <summary>
    /// Find the left edge from a vertex.
    /// </summary>
    /// <returns></returns>
    public readonly int FindLeftEdge(int v)
    {
        var vert = Vertexs[v];
        var level = vert.Yp;
        var xpos = vert.Xp;
        bool? lastRelation = null;

        var crr = v;
        while (true)
        {
            var next = Edges[crr][0].Id;
            var newLevel = Vertexs[next].Yp;
            var newRelation = newLevel < level;

            crr = next;

            lastRelation ??= newRelation;
            if (lastRelation == newRelation)
                continue;
            
            lastRelation = newRelation;
            if (Vertexs[next].Xp > xpos)
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
        var midPoint = GetMidPoint(ref Vertexs[v], ref Vertexs[u]);

        for (int i = 0; i < Faces.Count; i++)
        {
            if (IsInFace(midPoint.x, midPoint.y, i))
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
            int k = (i + 2) % face.Count;

            if (Left(ref Vertexs[i], ref Vertexs[j], ref Vertexs[k]) < 0)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Create a new empty face.
    /// </summary>
    int CreateFace()
    {
        var id = nextEdgeId;
        nextEdgeId++;

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
        var edges = GetEdgeList(id);
        edges.Add(edge);

        var faceEdges = GetFaceEdgeList(id);
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
    /// The left operation. https://en.wikipedia.org/wiki/Left_and_right_(algebra)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float Left(ref PlanarVertex p, ref PlanarVertex q, ref PlanarVertex r)
    {
        var vx = p.Xp - q.Xp;
        var vy = p.Yp - q.Yp;
        
        var ux = r.Xp - q.Xp;
        var uy = r.Yp - q.Yp;

        return vx * uy - ux * vy;
    }

    /// <summary>
    /// Get the mid point between two points.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static (float x, float y) GetMidPoint(ref PlanarVertex p, ref PlanarVertex q)
        => ((p.Xp + q.Xp) / 2, (p.Yp + q.Yp) / 2);
}