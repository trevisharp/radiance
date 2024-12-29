/* Author:  Leonardo Trevisan Silio
 * Date:    29/12/2024
 */
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
        {
            // TODO
        }

        return MonotonePlaneTriangulation(dcel, points, sweepLine);
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

        for (int i = 0; i < sweepLine.Length; i++)
        {
            ref var v = ref sweepLine[i];
            var type = dcel.DiscoverType(v.Id);
            System.Console.WriteLine($"{v.Xp} {v.Yp}");
            System.Console.WriteLine(type);

            switch (type)
            {
                case VertexType.Start:
                    break;
                    
                case VertexType.End:
                    break;

                case VertexType.Split:
                    break;

                case VertexType.Merge:
                    break;

                case VertexType.Regular:
                    break;
            }
        }
        return false;
    }

    /// <summary>
    /// Receveing a map of ordenation and data with format (x, y, z, ...),
    /// if the points represetns a monotone polygon, return the triangularization
    /// of then.
    /// </summary>
    static float[] MonotonePlaneTriangulation(DCEL dcel, Span<PlanarVertex> points, SweepLine sweepLine)
    {
        var index = 0;
        int expectedTriangules = points.Length - 2;
        var triangules = new float[9 * expectedTriangules];

        var stack = new Stack<(int id, bool chain)>();
        stack.Push((sweepLine[0].Id, false));
        stack.Push((sweepLine[1].Id, true));

        for (int k = 2; k < points.Length; k++)
        {
            ref var crrIndex = ref sweepLine[k];
            var last = stack.Pop();
            var isConn = dcel.IsConnected(last.id, crrIndex.Id);
            (int id, bool chain) mid, next = (crrIndex.Id, !(isConn ^ last.chain));
            
            if (isConn)
            {
                do
                {
                    if (stack.Count == 0)
                    {
                        stack.Push(last);
                        stack.Push(next);
                        break;
                    }
                    
                    mid = last;
                    last = stack.Pop();
                    
                    if (left(points, last.id, mid.id, next.id) < 0)
                    {
                        stack.Push(last);
                        stack.Push(mid);
                        stack.Push(next);
                        break;
                    }
                    
                    dcel.Connect(last.id, next.id);
                    addTriangule(
                        points[last.id],
                        points[mid.id],
                        points[next.id]
                    );
                } while (true);

                continue;
            }
            
            var top = last;
            mid = stack.Pop();
            dcel.Connect(last.id, next.id);
            addTriangule(
                points[last.id],
                points[mid.id],
                points[next.id]
            );

            while (stack.Count > 0)
            {
                last = mid;
                mid = stack.Pop();
                dcel.Connect(last.id, next.id);
                addTriangule(
                    points[last.id],
                    points[mid.id],
                    points[next.id]
                );
            }
            stack.Push(top);
            stack.Push(next);
        }

        if (stack.Count > 2)
        {
            addTriangule(
                points[stack.Pop().id],
                points[stack.Pop().id],
                points[stack.Pop().id]
            );
        }

        return triangules;

        /// <summary>
        /// Add trinagule (p, q, r) to list of triangules data
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void addTriangule(PlanarVertex p, PlanarVertex q, PlanarVertex r)
        {
            triangules[index++] = p.X;
            triangules[index++] = p.Y;
            triangules[index++] = p.Z;
            
            triangules[index++] = q.X;
            triangules[index++] = q.Y;
            triangules[index++] = q.Z;
            
            triangules[index++] = r.X;
            triangules[index++] = r.Y;
            triangules[index++] = r.Z;
        }

        /// <summary>
        /// Teste if the r is left from (p, q) line 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        float left(Span<PlanarVertex> points, int pi, int qi, int ri)
        {
            ref var p = ref points[pi];
            ref var q = ref points[qi];
            ref var r = ref points[ri];

            var vx = p.Xp - q.Xp;
            var vy = p.Yp - q.Yp;
            
            var ux = r.Xp - q.Xp;
            var uy = r.Yp - q.Yp;

            return vx * uy - ux * vy;
        }
    }
}