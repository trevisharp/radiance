/* Author:  Leonardo Trevisan Silio
 * Date:    29/12/2024
 */
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Radiance.Internal;

/// <summary>
/// Represents a Double Connected Edge List.
/// </summary>
public readonly ref struct DCEL
{
    public readonly Span<PlanarVertex> Vertexs;
    public readonly Dictionary<int, List<int>> Edges = [];

    public DCEL(Span<PlanarVertex> points)
    {
        Vertexs = points;

        Edges.Add(0, [ points.Length - 1, 1 ]);
        Edges.Add(points.Length - 1, [ points.Length - 2, 0 ]);

        for (int i = 1; i < points.Length - 1; i++)
            Edges.Add(i, [ i - 1, i + 1]);
    }

    /// <summary>
    /// Receiving 2 ids for vertex return if them are connected.
    /// </summary>
    public bool IsConnected(int v, int u)
    {
        if (v == u)
            return false;
        
        return Edges[v].Contains(u);
    }

    /// <summary>
    /// Add a Edge between two vertex.
    /// </summary>
    public void Connect(int v, int u)
    {
        if (v == u)
            return;
        
        Edges[v].Add(u);
        Edges[v].Add(u);
        Edges[u].Add(v);
        Edges[u].Add(v);
    }

    /// <summary>
    /// Discover the type of the vertex with specific id.
    /// </summary>
    public VertexType DiscoverType(int v)
    {
        var edges = Edges[v];
        ref var self = ref Vertexs[v];
        ref var e1 = ref Vertexs[edges[0]];
        ref var e2 = ref Vertexs[edges[1]];
        
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
            var next = Edges[crr][0];
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
}