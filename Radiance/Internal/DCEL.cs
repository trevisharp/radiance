/* Author:  Leonardo Trevisan Silio
 * Date:    29/12/2024
 */
using System;
using System.Collections.Generic;

namespace Radiance.Internal;

/// <summary>
/// Represents a Double Connected Edge List.
/// </summary>
public readonly ref struct DCEL
{
    readonly Span<PlanarVertex> Vertex;
    public readonly Dictionary<int, List<int>> Edges = [];

    public DCEL(Span<PlanarVertex> points)
    {
        Vertex = points;

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
        Edges[u].Add(v);
    }

    /// <summary>
    /// Discover the type of the vertex with specific id.
    /// </summary>
    public VertexType DiscoverType(int v)
    {
        throw new NotImplementedException();
    }
}