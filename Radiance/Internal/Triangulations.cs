/* Author:  Leonardo Trevisan Silio
 * Date:    29/08/2024
 */
using System.Collections.Generic;

namespace Radiance.Internal;

/// <summary>
/// A class that contains some util and opeartions.
/// </summary>
public static class Triangulations
{
    private const int sortTreshold = 8;

    /// <summary>
    /// Find a plane (ax + by + cz + d = 0) that better match the points
    /// </summary>
    public static (float a, float b, float c, float d) PlaneRegression(float[] pts)
    {
        /*
        E = S (a*x_i + b*y_i + c*z_i + d)^2 / N
          
          = (a^2 S x_i^2 + b^2 S y_i^2 + c^2 S z_i^2 + N d^2
          + ab S x_i y_i + ac S x_i z_i + ad S x_i
          + ba S x_i y_i + bc S y_i z_i + bd S y_i
          + ca S x_i z_i + cb S y_i z_i + cd S z_i
          + da S x_i + db S y_i + cd S z_i) / N
          
          = (a^2 Sxx + b^2 Syy + c^2 Szz) / N + d^2
          + (2ab Sxy + 2ac Sxz + 2ad Sx) / N
          + (2bc Syz + 2bd Sy) / N
          + (2cd Sz) / N

          = a^2 qx + b^2 qy + c^2 qz
          + 2ab pxy + 2ac pxz + 2bc pyz 
          + 2ad xm + 2bd ym + 2cd zm
          + d^2

          qa = S a_i^2 / N
          pab = S a_i b_i / N
          am = S a_i / N
         */

        int N = pts.Length / 3;
        float qx = 0f, qy = 0f, qz = 0f,
              pxy = 0f, pyz = 0f, pxz = 0f,
              xm = 0f, ym = 0f, zm = 0f;
        
        for (int i = 0; i < pts.Length; i += 3)
        {
            float x = pts[i + 0],
                  y = pts[i + 1],
                  z = pts[i + 2];
            
            qx += x * x;
            qy += y * y;
            qz += z * z;
            pxy += x * y;
            pyz += y * z;
            pxz += x * z;
            xm += x;
            ym += y;
            zm += z;
        }
        qx /= N;
        qy /= N;
        qz /= N;
        pxy /= N;
        pyz /= N;
        pxz /= N;
        xm /= N;
        ym /= N;
        zm /= N;

        /*
        f(a, b, c, d) =
            a^2 qx + b^2 qy + c^2 qz
            + 2ab pxy + 2ac pxz + 2bc pyz 
                + 2ad xm + 2bd ym + 2cd zm
                + d^2

        df/da = 2a qx + 2b pxy + 2c pxz + 2dxm = 0
        df/db = 2b qy + 2a pxy + 2c pyz + 2d ym = 0
        df/dc = 2c qz + 2a pxz + 2b pyz + 2d zm = 0
        df/dd = 2a xm + 2b ym + 2c zm + 2d = 0

        a qx  + b pxy + c pxz + d xm = 0
        a pxy + b qy  + c pyz + d ym = 0
        a pxz + b pyz + c qz  + d zm = 0
        a xm  + b ym  + c zm  + d    = 0

        |a| |qx  pxy pxz xm|   |0|
        |b| |pxy qy  pyz ym| = |0|
        |c| |pxz pyz qz  zm|   |0|
        |d| |xm  ym  zm  1 |   |0|
         */
        float w = 1f / (qx + qy + qz),
            a = 1f, b = 1f, c = 1f, d = 1f,
            da = qx + pxy + pxz + xm,
            db = qy + pxy + pyz + ym,
            dc = qz + pxz + pyz + zm,
            dd = xm + ym + zm + 1;
        
        for (int i = 0; i < 100; i++)
        {
            a -= w * da;
            b -= w * db;
            c -= w * dc;
            d -= w * dd;

            da = a * qx  + b * pxy + c * pxz + d * xm;
            db = a * pxy + b * qy  + c * pyz + d * ym;
            dc = a * pxz + b * pyz + c * qz  + d * zm;
            dd = a * xm  + b * ym  + c * zm  + d;

            var mod = da * da + db * db + dc * dc + dd * dd;
            if (mod < 0.1f)
                break;
        }

        return (a, b, c, d);
    }
    
    /// <summary>
    /// Get a triangulation of a polygon with points in a
    /// clockwise order.
    /// </summary>
    public static float[] PlanarPolygonTriangulation(float[] pts)
    {
        var N = pts.Length / 3;
        if (N < 4)
            return pts;
        
        var triangules = new List<float>();
        
        var plane = PlaneRegression(pts);
        var points = ToPlanarPoints(pts, plane);
        var orderMap = Sort(points, 5, 3, 4);
        var edges = new PolygonEdgeCollection(N);
        var status = new OrderedEdgeCollection(points, 3, 4);
        var visited = new bool[N];
        var helper = new Dictionary<(int, int), int>();
        
        // TODO
        // monotone subdivision
        
        MonotonePlaneTriangulation(orderMap, 
            points, triangules, 5
        );
        return [.. triangules];
    }

    static void MonotonePlaneTriangulation(
        int[] polyOrderMap, float[] data,
        List<float> triangules, int dataSize
    )
    {
        var edges = new PolygonEdgeCollection(polyOrderMap.Length);

        var stack = new Stack<(int index, bool chain)>();
        stack.Push((polyOrderMap[0], false));
        stack.Push((polyOrderMap[1], true));

        for (int k = 2; k < polyOrderMap.Length; k++)
        {
            var crrIndex = polyOrderMap[k];
            var last = stack.Pop();
            var isConn = edges.IsConnected(
                last.index / dataSize,
                crrIndex / dataSize
            );
            (int index, bool chain) next = (crrIndex, !(isConn ^ last.chain));
            
            if (isConn)
            {
                (int index, bool chain) mid;
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
                    if (left(last.index, mid.index, next.index) < 0)
                    {
                        stack.Push(last);
                        stack.Push(mid);
                        stack.Push(next);
                        break;
                    }
                    
                    edges.Connect(
                        last.index / dataSize,
                        next.index / dataSize
                    );
                    addTriangule(last.index, mid.index, next.index);
                } while (true);
            }
            else
            {
                var top = last;
                var mid = stack.Pop();
                edges.Connect(
                    last.index / dataSize,
                    next.index / dataSize
                );
                addTriangule(last.index, mid.index, next.index);

                while (stack.Count > 0)
                {
                    last = mid;
                    mid = stack.Pop();
                    edges.Connect(
                        last.index / dataSize,
                        next.index / dataSize
                    );
                    addTriangule(last.index, mid.index, next.index);
                }
                stack.Push(top);
                stack.Push(next);
            }
        }
        if (stack.Count > 2)
        {
            int a = stack.Pop().index,
                b = stack.Pop().index,
                c = stack.Pop().index;
            addTriangule(a, b, c);
        }

        /// <summary>
        /// Add trinagule (p, q, r) to list of triangules data
        /// </summary>
        void addTriangule(int p, int q, int r)
        {
            addPoint(p);
            addPoint(q);
            addPoint(r);
        }

        /// <summary>
        /// Add point p to list of triangules data 
        /// </summary>
        void addPoint(int p)
        {
            triangules.Add(data[p + 0]);
            triangules.Add(data[p + 1]);
            triangules.Add(data[p + 2]);
        }

        /// <summary>
        /// Teste if the r is left from (p, q) line 
        /// </summary>
        float left(int p, int q, int r)
        {
            var vx = data[p + 3] - data[q + 3];
            var vy = data[p + 4] - data[q + 4];
            
            var ux = data[r + 3] - data[q + 3];
            var uy = data[r + 4] - data[q + 4];

            return vx * uy - ux * vy;
        }
    }

    static int[] Sort(float[] data, int size, int offsetA, int offsetB = -1)
    {
        var orderMap = new int[data.Length / size];
        for (int i = 0, n = 0; i < orderMap.Length; i++, n += size)
            orderMap[i] = n;

        QuickSort(data, offsetA, offsetB, size, orderMap, 0, orderMap.Length);

        return orderMap;
    }

    static void QuickSort(
        float[] data, int offsetA, int offsetB, int size, 
        int[] map, int start, int end
    )
    {
        int len = end - start;
        if (len < sortTreshold)
        {
            SlowSort(data, offsetA, offsetB, size, map, start, end);
            return;
        }

        var goodPivoIndex = start + len / 4;
        var pivoIndex = map[goodPivoIndex];
        var pivo = data[pivoIndex + offsetA];

        map[goodPivoIndex] = map[end - 1];
        map[end - 1] = pivoIndex;

        int i = start, j = end - 2;
        while (i < j)
        {
            float iv = data[map[i] + offsetA];
            while(iv < pivo && i < j)
                iv = data[map[++i] + offsetA];
            
            float jv = data[map[j] + offsetA];
            while (jv > pivo && i < j)
                jv = data[map[--j] + offsetA];

            if (i >= j)
                break;

            (map[j], map[i]) = (map[i], map[j]);
        }

        if (data[map[j] + offsetA] < pivo)
            j++;

        (map[end - 1], map[j]) = (map[j], map[end - 1]);
        QuickSort(data, offsetA, offsetB, size, map, start, j);
        QuickSort(data, offsetA, offsetB, size, map, j, end);
    }

    static void SlowSort(
        float[] data, int offsetA, int offsetB, int size, 
        int[] map, int start, int end
    )
    {
        bool sorted = false;
        while (!sorted)
        {
            sorted = true;
            for (int i = start; i < end - 1; i++)
            {
                int j = map[i],
                    k = map[i + 1];
                var v1 = data[j + offsetA];
                var v2 = data[k + offsetA];
                if (v1 < v2)
                    continue;

                if (v1 == v2)
                {
                    v1 = data[j + offsetB];
                    v2 = data[k + offsetB];
                    if (v1 <= v2)
                        continue;
                }
                
                map[i] = k;
                map[i + 1] = j;
                sorted = false;
            }
        }
    }
    
    static float[] ToPlanarPoints(float[] original, (float a, float b, float c, float d) plane)
    {
        var result = new float[5 * original.Length / 3];
        int n = 0;

        float a = plane.a,
              b = plane.b,
              c = plane.c,
              d = plane.d;
        var mod = a * a + b * b + c * c;

        var originDist = -d / mod;
        float xo = a * originDist,
              yo = b * originDist,
              zo = c * originDist;
        
        float A, B1, B2;
        int j, k;
        
        if (a != 0)
        {
            /**
            u = (-b, a, 0)
            v = (-c, 0, a)

            r * u + s * v + o = p
            r * a + yo = yp -> r = (yp - yo) / a
            s * a + zo = zp -> s = (zp - zo) / a
            **/

            A = 1 / a;
            B1 = -yo / a;
            B2 = -zo / a;
            j = 1;
            k = 2;
        }
        else if (b != 0)
        {
            /**
            u = (0, -c, b)
            v = (b, -a, 0)

            r * u + s * v + o = p
            r * b + xo = xp -> r = (xp - xo) / b
            s * b + zo = zp -> s = (zp - zo) / b
            **/

            A = 1 / b;
            B1 = -xo / b;
            B2 = -zo / b;
            j = 0;
            k = 2;
        }
        else // c != 0
        {
            /**
            u = (c, 0, -a)
            v = (0, c, -b)

            r * u + s * v + o = p
            r * c + xo = xp -> r = (xp - xo) / c
            s * c + yo = yp -> s = (yp - yo) / c
            **/

            A = 1 / c;
            B1 = -yo / c;
            B2 = -yo / c;
            j = 0;
            k = 1;
        }

        for (int i = 0; i < original.Length; i += 3)
        {
            float x = original[i + 0],
                  y = original[i + 1],
                  z = original[i + 2];

            result[n + 0] = x;
            result[n + 1] = y;
            result[n + 2] = z;

            /**
            a (x + a * t) + b (y + b * t) + c (z + c * t) + d = 0
            a x + a^2 t + b y + b^2 t + c z + c^2 t + d = 0
            t = -(ax + by + cz + d) / (a^2 + b^2 + c^2)

            xp = x + a * t
            yp = y + b * t
            zp = z + c * t
            p = (xp, yp, zp)
            **/
            var t = -(a * x + b * y + c * z + d) / mod;

            float xp = x + a * t,
                  yp = y + b * t,
                  zp = z + b * t;
            
            result[n + 3] = A * original[i + j] + B1;
            result[n + 4] = A * original[i + k] + B2;
            n += 5;
        }
        
        return result;
    }

    internal class PolygonEdgeCollection(int verticesCount)
    {
        readonly int last = verticesCount - 1;
        readonly List<int>[] list = new List<int>[verticesCount];

        internal List<int> GetConnections(int i)
        {
            Init(i);
            return list[i];
        }

        internal bool IsConnected(int i, int j)
        {
            if (i == j)
                return false;
            
            Init(i);
            return list[i].Contains(j);
        }

        internal void Connect(int i, int j)
        {
            Init(i);
            Init(j);
            list[i].Add(j);
            list[j].Add(i);
        }

        private void Init(int index)
        {
            if (list[index] is not null)
                return;
            
            list[index] = [
                index == last ? 0 : index + 1,
                index == 0 ? last : index - 1
            ];
        }
    }

    internal class OrderedEdgeCollection(
        float[] points,
        int xIndex,
        int yIndex)
    {
        readonly LinkedList<EdgeInfo> edges = [];

        internal (int i, int j) GetAbove(float x, float y)
        {
            var crr = edges.Last;

            while (crr is not null)
            {
                var edge = crr.Value;
                if (edge.a * x + edge.b >= y)
                    return (edge.vi, edge.vj);
                crr = crr.Previous;
            }

            var first = edges.First!;
            var value = first.Value;
            return (value.vi, value.vj);
        }

        internal void RemoveEdge(int vi, int vj)
        {
            foreach (var node in edges)
            {
                if (node.vi != vi || node.vj != vj)
                    continue;
                
                edges.Remove(node);
                return;
            }
        }

        internal bool Contains(int vi, int vj)
        {
            foreach (var node in edges)
            {
                if (node.vi != vi || node.vj != vj)
                    continue;
                
                return true;
            }
            return false;
        }

        internal void AddEdge(int vi, int vj)
        {
            var edge = ToEdge(vi, vj);
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

        EdgeInfo ToEdge(int vi, int vj)
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

    internal struct EdgeInfo
    {
        internal int vi;
        internal int vj;
        internal float x0;
        internal float x1;
        internal float a;
        internal float b;
        
        internal readonly bool IsAbove(EdgeInfo e)
        {
            float x = 
                x0 > e.x0 && x0 < e.x1 ?
                x0 : e.x0;
            
            float y = a * x + b;
            float ey = e.a * x + e.b;

            return y > ey;
        }
    }
}