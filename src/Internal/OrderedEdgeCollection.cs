/* Author:  Leonardo Trevisan Silio
 * Date:    01/10/2023
 */
using System.Collections.Generic;

namespace Radiance.Internal;

internal class OrderedEdgeCollection
{
    internal struct EdgeInfo
    {
        internal int vi;
        internal int vj;
        internal float x0;
        internal float x1;
        internal float a;
        internal float b;
        
        internal bool IsAbove(EdgeInfo e)
        {
            float x = 
                x0 > e.x0 && x0 < e.x1 ?
                x0 : e.x0;
            
            float y = a * x + b;
            float ey = e.a * x + e.b;

            return y > ey;
        }
    }

    float[] points;
    int xIndex;
    int yIndex;
    LinkedList<EdgeInfo> edges = new();

    internal OrderedEdgeCollection(
        float[] points,
        int xIndex,
        int yIndex
    )
    {
        this.points = points;
        this.xIndex = xIndex;
        this.yIndex = yIndex;
    }

    public (int i, int j) GetAbove(float x, float y)
    {
        var crr = edges.Last;

        while (crr is not null)
        {
            var edge = crr.Value;
            if (edge.a * x + edge.b >= y)
                return (edge.vi, edge.vj);
            crr = crr.Previous;
        }

        var first = edges.First.Value;
        return (first.vi, first.vj);
    }

    public void RemoveEdge(int vi, int vj)
    {
        foreach (var node in edges)
        {
            if (node.vi != vi || node.vj != vj)
                continue;
            
            edges.Remove(node);
            return;
        }
    }

    public bool Contains(int vi, int vj)
    {
        foreach (var node in edges)
        {
            if (node.vi != vi || node.vj != vj)
                continue;
            
            return true;
        }
        return false;
    }

    public void AddEdge(int vi, int vj)
    {
        var edge = toEdge(vi, vj);
        if (edges.Count == 0)
        {
            edges.AddFirst(edge);
            return;
        }

        var crr = edges.First;
        while (crr is not null)
        {
            if (edge.IsAbove(crr.Value))
                break;
            
            crr = crr.Next;
        }

        if (crr is null)
            edges.AddLast(edge);
        else edges.AddBefore(crr, edge);
    }

    private EdgeInfo toEdge(int vi, int vj)
    {
        var ix = points[vi + xIndex];
        var iy = points[vi + yIndex];

        var jx = points[vj + xIndex];
        var jy = points[vj + yIndex];

        var a = iy == jy ? 0 : (jy - iy) / (jx - ix);

        return new EdgeInfo
        {
            vi = vi,
            vj = vj,
            x0 = ix < jx ? ix : jx,
            x1 = ix < jx ? jx : ix,
            a = a,
            b = iy - a * ix
        };
    }

}