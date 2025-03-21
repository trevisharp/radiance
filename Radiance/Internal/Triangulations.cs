/* Author:  Leonardo Trevisan Silio
 * Date:    20/03/2025
 */
using System;
using System.Linq;
using System.Collections.Generic;

namespace Radiance.Internal;

/// <summary>
/// A class that contains some util and opeartions.
/// </summary>
public static class Triangulations
{
    /// <summary>
    /// Get a triangulation of a polygon with points in a
    /// clockwise order.
    /// </summary>
    public static float[] PlanarPolygonTriangulation(float[] pts)
    {
        var N = pts.Length / 3;
        if (N < 4)
            return pts;

        Span<PlanarVertex> points = 
            N < 2048 ?
            stackalloc PlanarVertex[N] :
            new PlanarVertex[N];
        PlanarVertex.ToPlanarVertex(pts, points);
        
        Span<int> map =
            N < 2048 ?
            stackalloc int[N] :
            new int[N];
        var sweepLine = SweepLine.Create(points, map);

        var dcel = new DCEL(points);

        if (MonotoneDivision(dcel, sweepLine))
            return NonMonotonePlaneTriangularization(dcel, sweepLine);
        
        return MonotonePlaneTriangulation(dcel, sweepLine);
    }
    
    /// <summary>
    /// Divide a polygon on many monotone polygons.
    /// Return true if some polygon has created.
    /// </summary>
    static bool MonotoneDivision(DCEL dcel, SweepLine sweepLine)
    {
        var types = new VertexType[sweepLine.Length];
        for (int i = 0; i < sweepLine.Length; i++)
            types[i] = dcel.DiscoverType(i);
        
        if (!types.Contains(VertexType.Merge) && !types.Contains(VertexType.Split))
            return false;
        
        // information if polygon lies right or left vertex
        int start = sweepLine[0].Id;
        int index = start + sweepLine.Length;
        var polygonRight = new bool[sweepLine.Length];
        polygonRight[(index - 1) % sweepLine.Length] = true;
        for (int i = 0; i < sweepLine.Length; i++, index++)
        {
            int crr = index % sweepLine.Length;
            int prev = (index - 1) % sweepLine.Length;
            polygonRight[crr] = polygonRight[prev];

            if (types[crr] == VertexType.Regular)
                continue;
            
            polygonRight[crr] = !polygonRight[crr];
        }

        HashSet<int> edgesCollect = [];
        Dictionary<int, int> helper = [];
        for (int i = 0; i < sweepLine.Length; i++)
            helper[i] = -1;
        
        for (int i = 0; i < sweepLine.Length; i++)
        {
            ref var v = ref sweepLine[i];
            var vi = v.Id;
            
            var type = types[vi];
            var edges = dcel.Edges[vi];
            var ei = edges[0].Id;
            var eprev = ei - 1;
            if (eprev == -1)
                eprev = sweepLine.Length - 1;

            switch (type)
            {
                case VertexType.Start:

                    edgesCollect.Add(eprev);
                    helper[eprev] = vi;

                    break;
                    
                case VertexType.End:

                    edgesCollect.Remove(ei);

                    if (helper[ei] == -1)
                        break;
                    
                    if (types[helper[ei]] != VertexType.Merge)
                        break;
                    
                    dcel.Connect(vi, helper[ei]);

                    break;

                case VertexType.Split:

                    var ej1 = dcel.FindLeftEdge(vi);
                    dcel.Connect(helper[ej1], vi);
                    helper[ej1] = vi;
                    edgesCollect.Add(ej1);

                    break;

                case VertexType.Merge:

                    if (helper[ei] != -1 && types[helper[ei]] == VertexType.Merge)
                    {
                        dcel.Connect(vi, helper[ei]);
                    }

                    edgesCollect.Remove(ei);

                    var ej2 = dcel.FindLeftEdge(vi);
                    if (helper[ej2] != -1 && types[helper[ej2]] == VertexType.Merge)
                    {
                        dcel.Connect(helper[ej2], vi);
                    }
                    helper[ej2] = vi;

                    break;

                case VertexType.Regular:

                    if (polygonRight[vi])
                    {
                        if (helper[ei] != -1 && types[helper[ei]] == VertexType.Merge)
                        {
                            dcel.Connect(vi, helper[ei]);
                        }

                        edgesCollect.Remove(ei);
                        edgesCollect.Add(eprev);
                        helper[eprev] = vi;
                    }
                    else
                    {
                        var ej3 = dcel.FindLeftEdge(vi);
                        if (helper[ej3] != -1 && types[helper[ej3]] == VertexType.Merge)
                        {
                            dcel.Connect(helper[ej3], vi);
                        }
                        helper[ej3] = vi;
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
    static float[] NonMonotonePlaneTriangularization(DCEL dcel, SweepLine sweepLine)
    {
        var index = 0;
        int expectedTriangules = dcel.Length - 2;
        var triangules = new float[9 * expectedTriangules];

        float[] data;
        while (dcel.Faces.Count > 0)
        {
            var subPointsIds = dcel.RemoveSubPolygon();   
            var subDcel = dcel.ApplyFilter(subPointsIds);
            if (subDcel.Length < 4)
            {
                data = subDcel.ToArray();
                Array.Copy(data, 0, triangules, index, data.Length);
                index += data.Length;
                continue;
            }
            
            var subSweepLine = sweepLine.ApplyFilter(subPointsIds);
            data = MonotonePlaneTriangulation(subDcel, subSweepLine);
            Array.Copy(data, 0, triangules, index, data.Length);
            index += data.Length;
        }

        return triangules;
    }

    /// <summary>
    /// Receveing a map of ordenation and data with format (x, y, z, ...),
    /// if the points represetns a monotone polygon, return the triangularization
    /// of then.
    /// </summary>
    static float[] MonotonePlaneTriangulation(DCEL dcel, SweepLine sweepLine)
    {
        var temp = dcel.FacesEdges.FirstOrDefault();
        var pts = temp.Value.SelectMany(x => new int[] { x.To, x.From }).Distinct();
        Console.WriteLine($"MonotonePlaneTriangulation({string.Join(", ", pts)})");

        var chainA = new Stack<int>();
        chainA.Push(sweepLine[0].Id);
        chainA.Push(sweepLine[1].Id);
        bool leftChain = sweepLine[0].X < sweepLine[1].X;

        for (int k = 2; k < dcel.Length; k++)
        {
            var nextId = sweepLine[k].Id;
            if (dcel.IsConnected(chainA.Peek(), nextId))
                chainA.Push(nextId);
        }

        var stack = new Stack<int>();
        stack.Push(sweepLine[0].Id);
        stack.Push(sweepLine[1].Id);

        for (int k = 2; k < dcel.Length - 1; k++)
        {
            var topId = stack.Pop();
            var nextId = sweepLine[k].Id;

            var topInChainA = chainA.Contains(topId);
            var nextInChainA = chainA.Contains(nextId);
            var sameChain = topInChainA == nextInChainA;

            if (sameChain)
            {
                var currId = -1;
                var midId = topId;
                while (stack.Count > 0)
                {
                    currId = stack.Pop();
                    ref var middle = ref dcel.GetVertex(midId);
                    ref var top = ref dcel.GetVertex(currId);
                    ref var next = ref dcel.GetVertex(nextId);

                    var cross = (middle.X - top.X) * (next.Y - top.Y) - (middle.Y - top.Y) * (next.X - top.X);
                    var canConnect = 
                        nextInChainA && leftChain && cross > 0 ||
                        nextInChainA && !leftChain && cross < 0 ||
                        !nextInChainA && leftChain && cross > 0 ||
                        !nextInChainA && !leftChain && cross < 0;

                    if (!canConnect)
                        break;
                    dcel.Connect(nextId, currId);
                    midId = currId;
                }

                if (midId == topId)
                    stack.Push(midId);
                stack.Push(currId);
                stack.Push(nextId);
            }
            else
            {
                dcel.Connect(nextId, topId);
                while (stack.Count > 0)
                {
                    var currId = stack.Pop();
                    dcel.Connect(nextId, currId);
                }
                stack.Push(topId);
                stack.Push(nextId);
            }


        }

        return dcel.ToArray();
    }
}