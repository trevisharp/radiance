/* Author:  Leonardo Trevisan Silio
 * Date:    26/03/2025
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

        var (leftChain, rightChain) = dcel.GetChains(sweepLine);

        Dictionary<int, int> helper = [];
        
        for (int i = 0; i < sweepLine.Length; i++)
        {
            ref var v = ref sweepLine[i];
            var vi = v.Id;
            
            var type = types[vi];
            var edges = dcel.VertexEdges[vi];
            var ei = edges[0].Id;
            var eprev = ei - 1;
            if (eprev == -1)
                eprev = sweepLine.Length - 1;

            System.Console.WriteLine(vi);
            Console.WriteLine($"T = {string.Join(", ", helper.Select(p => (p.Key, p.Value)))}");
            switch (type)
            {
                case VertexType.Start:

                    helper[eprev] = vi;

                    break;
                    
                case VertexType.End:
                    
                    if (types[helper[ei]] == VertexType.Merge)
                    {
                        dcel.Connect(vi, helper[ei]);
                    }
                    
                    helper.Remove(ei);

                    break;

                case VertexType.Split:

                    var ej1 = dcel.FindLeftEdge(vi);
                    dcel.Connect(helper[ej1], vi);
                    helper[ej1] = vi;

                    break;

                case VertexType.Merge:

                    if (types[helper[ei]] == VertexType.Merge)
                    {
                        dcel.Connect(vi, helper[ei]);
                    }

                    helper.Remove(ei);

                    var ej2 = dcel.FindLeftEdge(vi);
                    if (types[helper[ej2]] == VertexType.Merge)
                    {
                        dcel.Connect(helper[ej2], vi);
                    }
                    helper[ej2] = vi;

                    break;

                case VertexType.Regular:

                    if (leftChain.Contains(vi))
                    {
                        if (types[helper[ei]] == VertexType.Merge)
                        {
                            dcel.Connect(vi, helper[ei]);
                        }

                        helper.Remove(ei);
                        helper[eprev] = vi;
                    }
                    else
                    {
                        var ej3 = dcel.FindLeftEdge(vi);
                        if (types[helper[ej3]] == VertexType.Merge)
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
        Console.WriteLine($"MonotonePlaneTriangulation({string.Join(",", pts)})");

        var (leftChain, rightChain) = dcel.GetChains(sweepLine);
        
        System.Console.WriteLine(string.Join(",", leftChain));
        System.Console.WriteLine(string.Join(",", rightChain));

        var stack = new Stack<int>();
        stack.Push(sweepLine[0].Id);
        stack.Push(sweepLine[1].Id);

        for (int j = 2; j < dcel.Length - 1; j++)
        {
            var vtop = stack.Peek();
            var vj = sweepLine[j].Id;

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
                var vj_1 = sweepLine[j - 1].Id;
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

        var vn = sweepLine[^1].Id;
        stack.Pop();

        while (stack.Count > 1)
        {
            var vk = stack.Pop();
            dcel.Connect(vk, vn);
        }

        return dcel.ToArray();
    }
}