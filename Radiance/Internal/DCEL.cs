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
public readonly ref struct DCEL
{
    readonly int[] nextEdgeId = [ 0 ];
    readonly int[] nextFaceId = [ 1 ];
    public readonly Span<PlanarVertex> Vertexs;
    public readonly Dictionary<int, List<HalfEdge>> Edges = [];
    public readonly Dictionary<int, List<int>> Faces = [];

    public DCEL(Span<PlanarVertex> points)
    {
        Vertexs = points;

        HalfEdge fst, prv;
        fst = prv = CreateEdge(0, 1);

        int i = 1;
        while (i < points.Length - 1)
        {
            var crr = CreateEdge(i, i + 1);
            crr.SetPrevious(prv);

            prv = crr;
            i++;
        }

        var lst = new HalfEdge(i, i, 0);
        lst.SetPrevious(prv);
        lst.SetNext(fst);
        Edges.Add(i, [ lst ]);

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
        
        List<int> faceA = [];
        List<int> faceB = [];

        var edge = Edges[v];
        faceA.Add(v);
        

        var e1 = CreateEdge(v, u);
        var e2 = CreateEdge(u, v);
    }

    /// <summary>
    /// Discover the type of the vertex with specific id.
    /// </summary>
    public VertexType DiscoverType(int v)
    {
        var edges = Edges[v];
        ref var self = ref Vertexs[v];
        ref var e1 = ref Vertexs[edges[0].Id];
        ref var e2 = ref Vertexs[edges[1].Id];
        
        if (over(ref self, ref e1) && over(ref self, ref e2))
            return left(ref e1, ref self, ref e2) < 0 ?
                VertexType.Split : VertexType.Start;
        
        if (over(ref e1, ref self) && over(ref e2, ref self))
            return left(ref e1, ref self, ref e2) < 0 ?
                VertexType.Merge : VertexType.End;

        return VertexType.Regular;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool over(ref PlanarVertex p, ref PlanarVertex q)
            => p.Yp > q.Yp || (p.Yp == q.Yp && p.Xp > q.Xp);
            
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        float left(ref PlanarVertex p, ref PlanarVertex q, ref PlanarVertex r)
        {
            var vx = p.Xp - q.Xp;
            var vy = p.Yp - q.Yp;
            
            var ux = r.Xp - q.Xp;
            var uy = r.Yp - q.Yp;

            return vx * uy - ux * vy;
        }
    }

    /// <summary>
    /// Find the left edge from a vertex.
    /// </summary>
    /// <returns></returns>
    public int FindLeftEdge(int v)
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

    HalfEdge CreateEdge(int to, int from)
    {
        var id = nextEdgeId[0];
        nextEdgeId[0]++;

        var edge = new HalfEdge(id, to, from);
        Edges[to].Add(edge);

        return edge;
    }
}