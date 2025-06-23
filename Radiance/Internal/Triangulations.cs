/* Author:  Leonardo Trevisan Silio
 * Date:    12/06/2025
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

        var dcel = new DCEL(pts);
        return PlanarPolygonTriangulation(dcel);
    }
    
    /// <summary>
    /// Get a triangulation of a polygon with points in a
    /// clockwise order.
    /// </summary>
    public static float[] PlanarPolygonTriangulation(DCEL dcel)
    {
        var sweepLine = dcel.CreateSweepLine();

        if (MonotoneDivision(dcel, sweepLine))
            return NonMonotonePlaneTriangularization(dcel, sweepLine);
        
        return MonotonePlaneTriangulation(dcel, sweepLine);
    }

    /// <summary>
    /// Divide a polygon on many monotone polygons.
    /// Return true if some polygon has created.
    /// </summary>
    public static bool MonotoneDivision(DCEL dcel, SweepLine sweepLine)
    {
        if (dcel.IsMonotone)
            return false;

        Dictionary<int, int> helper = [];
        
        for (int i = 0; i < sweepLine.Length; i++)
        {
            var v = sweepLine[i];
            var vi = v.Id;
            
            var type = dcel.GetVertexType(vi);
            var ei = dcel.FromEdgeMap[vi][0].Id;
            var eprev = dcel.ToEdgeMap[vi][0].Id;
            
            switch (type)
            {
                case VertexType.Start:

                    helper[ei] = vi;

                    break;
                    
                case VertexType.End:
                    
                    if (dcel.GetVertexType(helper[eprev]) == VertexType.Merge)
                    {
                        dcel.Connect(vi, helper[eprev]);
                    }

                    break;

                case VertexType.Split:

                    var ej1 = dcel.FindLeftEdge(vi);
                    dcel.Connect(helper[ej1], vi);
                    helper[ej1] = vi;
                    helper[ei] = vi;

                    break;

                case VertexType.Merge:

                    if (dcel.GetVertexType(helper[eprev]) == VertexType.Merge)
                    {
                        dcel.Connect(vi, helper[eprev]);
                    }
                    
                    var ej2 = dcel.FindLeftEdge(vi);
                    if (dcel.GetVertexType(helper[ej2]) == VertexType.Merge)
                    {
                        dcel.Connect(helper[ej2], vi);
                    }
                    
                    helper[ej2] = vi;

                    break;

                case VertexType.Regular:

                    if (dcel.LiesOnRight(vi))
                    {
                        if (dcel.GetVertexType(helper[eprev]) == VertexType.Merge)
                        {
                            dcel.Connect(vi, helper[eprev]);
                        }

                        helper[ei] = vi;
                    }
                    else
                    {
                        var ej3 = dcel.FindLeftEdge(vi);
                        if (dcel.GetVertexType(helper[ej3]) == VertexType.Merge)
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
    public static float[] NonMonotonePlaneTriangularization(DCEL dcel, SweepLine sweepLine)
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
            
            var subSweepLine = subDcel.CreateSweepLine();
            data = MonotonePlaneTriangulation(subDcel, subSweepLine);
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
    public static float[] MonotonePlaneTriangulation(DCEL dcel, SweepLine sweepLine)
    {
        var (leftChain, rightChain) = dcel.GetChains(sweepLine);

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