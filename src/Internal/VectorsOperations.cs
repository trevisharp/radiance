/* Author:  Leonardo Trevisan Silio
 * Date:    09/10/2023
 */
using System;
using System.Linq;
using System.Collections.Generic;

using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics.Arm;

namespace Radiance.Internal;

/// <summary>
/// Contains operations to transform vectors data
/// </summary>
internal static class VectorsOperations
{
    private const int sortTreshold = 32;

    internal static (float a, float b, float c, float d) PlaneRegression(float[] pts)
    {
        /**
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
        **/

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

        var covxy = pxy - xm * ym;
        var covyz = pyz - ym * zm;
        var covxz = pxz - xm * zm;
        var varx = qx - xm * xm;
        var vary = qy - ym * ym;
        var varz = qz - zm * zm;
        
        var a = (covyz - covxy * covxz) / varx;
        var b = (covxz - covxy * covyz) / vary;
        var d = -a * xm - b * ym - zm;

        return (a, b, 1f, d);
    }

    /// <summary>
    /// Get a triangulation of a polygon with points in a
    /// clockwise order.
    /// </summary>
    internal static float[] PlanarPolygonTriangulation(float[] pts)
    {
        var N = pts.Length / 3;
        if (N < 4)
            return pts;
        
        var triangules = new List<float>();
        
        var plane = PlaneRegression(pts);
        var points = toPlanarPoints(pts, plane);
        var orderMap = sort(points, 5, 3, 4);
        var edges = new PolygonEdgeCollection(N);
        var status = new OrderedEdgeCollection(points, 3, 4);
        var visited = new bool[N];
        var helper = new Dictionary<(int, int), int>();
        
        // TODO
        // for (int i = 0; i < N; i++)
        // {
        //     var k = orderMap[i];
        //     visited[k] = true;

        // }

        // foreach (var poly in edges.GetPolygons())
        
        monotonePlaneTriangulation(orderMap, 
            points, triangules, 5
        );
        return triangules.ToArray();

        void treatSplit(int vertex)
        {
            var conns = edges.GetConnections(vertex);
            float x = points[vertex + 3];
            float y = points[vertex + 4];

            var above = status.GetAbove(x, y);
            var help = helper[(above.i, above.j)];
            edges.Connect(help, vertex);

            foreach (var conn in conns)
            {
                status.AddEdge(vertex, conn);
                helper[(vertex, conn)] = vertex;
            }
        }

        void treatMerge(int vertex)
        {
            var conns = edges.GetConnections(vertex);
            float x = points[vertex + 3];
            float y = points[vertex + 4];

            var above = status.GetAbove(x, y);
            foreach (var conn in conns)
                status.RemoveEdge(conn, vertex);
            helper[above] = vertex;
        }

        void treatStart(int vertex)
        {
            var conns = edges.GetConnections(vertex);
            foreach (var conn in conns)
                status.AddEdge(vertex, conn);
            helper[(vertex, conns.First())] = vertex;
        }

        void treatEnd(int vertex)
        {
            var conns = edges.GetConnections(vertex);
            foreach (var conn in conns)
                status.RemoveEdge(vertex, conn);
            edges.Connect(vertex, conns.First());
        }
    }

    private static void monotonePlaneTriangulation(
        int[] polyOrderMap, float[] data,
        List<float> triangules, int dataSize
    )
    {
        var edges = new PolygonEdgeCollection(polyOrderMap.Length);

        var stack = new Stack<int>();
        stack.Push(polyOrderMap[0]);
        stack.Push(polyOrderMap[1]);

        for (int k = 2; k < polyOrderMap.Length; k++)
        {
            stack.Push(polyOrderMap[k]);
            while (stack.Count > 2)
            {
                var r = stack.Pop();
                var q = stack.Pop();
                var p = stack.Pop();

                var res = isTriangule(p, q, r);
                if (!res.HasValue)
                {
                    stack.Push(p);
                    stack.Push(q);
                    stack.Push(r);
                    break;
                }
                
                var diagonal = res.Value;
                edges.Connect(
                    diagonal.s / dataSize,
                    diagonal.t / dataSize
                );
                triangules.Add(data[p + 0]);
                triangules.Add(data[p + 1]);
                triangules.Add(data[p + 2]);
                triangules.Add(data[q + 0]);
                triangules.Add(data[q + 1]);
                triangules.Add(data[q + 2]);
                triangules.Add(data[r + 0]);
                triangules.Add(data[r + 1]);
                triangules.Add(data[r + 2]);

                if (stack.Count > 2)
                {
                    stack.Push(diagonal.s);
                    continue;
                }

                stack.Push(diagonal.s);
                stack.Push(diagonal.t);
            }
        }

        /// <summary>
        /// If p and q is connected test if (p, q, r) are a triangule
        /// and return the right face
        /// </summary>
        (int s, int t)? isTriangule(int p, int q, int r)
        {
            if (edges.IsConnected(q / dataSize, r / dataSize))
            {
                int mid = q;
                int aft, bef;
                if (p < mid && mid < r)
                {
                    bef = p;
                    aft = r;
                }
                else if (r < mid && mid < p)
                {
                    bef = r;
                    aft = p;
                }
                else
                {
                    bef = int.Max(p, r);
                    aft = int.Min(p, r);
                }

                var lft = left(bef, mid, aft);
                if (lft < 0)
                    return null;
                
                return (p, r);
            }
            else if (edges.IsConnected(p / dataSize, r / dataSize))
            {
                int mid = p;
                int aft, bef;
                if (q < mid && mid < r)
                {
                    bef = q;
                    aft = r;
                }
                else if (r < mid && mid < q)
                {
                    bef = r;
                    aft = q;
                }
                else
                {
                    bef = int.Max(q, r);
                    aft = int.Min(q, r);
                }

                var lft = left(bef, mid, aft);
                if (lft < 0)
                    return null;
                
                return (q, r);
            }
            return null;
        }

        float left(int p, int q, int r)
        {
            var vx = data[p + 3] - data[q + 3];
            var vy = data[p + 4] - data[q + 4];
            
            var ux = data[r + 3] - data[q + 3];
            var uy = data[r + 4] - data[q + 4];

            return vx * uy - ux * vy;
        }
    }

    private static int[] sort(float[] data, int size, int offsetA, int offsetB = -1)
    {
        var orderMap = new int[data.Length / size];
        for (int i = 0, n = 0; i < orderMap.Length; i++, n += size)
            orderMap[i] = n;

        quickSort(data, offsetA, offsetB, size, orderMap, 0, orderMap.Length);

        return orderMap;
    }

    // TODO: Fix bug in quickSort
    private static void quickSort(
        float[] data, int offsetA, int offsetB, int size, 
        int[] map, int start, int end
    )
    {
        if (end - start < sortTreshold)
        {
            slowSort(data, offsetA, offsetB, size, map, start, end);
            return;
        }

        var pivoIndex = map[end - 1];
        var pivo = data[pivoIndex + offsetA];

        int i = start, j = end - 2;
        while (i < j)
        {
            float iv = data[map[i]];
            while(iv < pivo && i < j)
                iv = data[map[++i]];
            
            float jv = data[map[j]];
            while (jv > pivo && i < j)
                jv = data[map[--j]];

            if (i >= j)
                break;
            
            var temp = map[i];
            map[i] = map[j];
            map[j] = temp;
        }
        map[end - 1] = map[j];
        map[j] = pivoIndex;

        quickSort(data, offsetA, offsetB, size, map, start, j);
        quickSort(data, offsetA, offsetB, size, map, j + 1, end);
    }

    private static void slowSort(
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
                    if (v1 < v2)
                        continue;
                }
                
                map[i] = k;
                map[i + 1] = j;
                sorted = false;
            }
        }
    }
    
    private static float[] toPlanarPoints(float[] original, (float a, float b, float c, float d) plane)
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

    private static (float a, float b, float c) solve3x3System(
        float A1, float B1, float C1, float K1,
        float A2, float B2, float C2, float K2,
        float A3, float B3, float C3, float K3
    )
    {
        if (A1 != 0)
        {
            /**
            (1) a = -(K1 + b B1 + c C1) / A1
            
            b B2 + c C2 - A2 / A1 * (K1 + b B1 + c C1) + K2 = 0
            b B3 + c C3 - A3 / A1 * (K1 + b B1 + c C1) + K3 = 0

            b (B2 - B1 * A2 / A1) + c (C2 - C1 * A2 / A1) - K1 * A2 / A1 + K2 = 0
            b (B3 - B1 * A3 / A1) + c (C3 - C1 * A3 / A1) - K1 * A3 / A1 + K3 = 0
            **/
            (float b, float c) = solve2x2System(
                B2 - B1 * A2 / A1, C2 - C1 * A2 / A1, -K1 * A2 / A1 + K2,
                B3 - B1 * A3 / A1, C3 - C1 * A3 / A1, -K1 * A3 / A1 + K3
            );
            var a =  -(K1 + b * B1 + c * C1) / A1;
            return (a, b, c);
        }
        else if (A2 != 0)
        {
            /**
            (1) a = -(K2 + b B2 + c C2) / A2
            
            b B1 + c C1 - A1 / A2 * (K2 + b B2 + c C2) + K1 = 0
            b B3 + c C3 - A3 / A2 * (K2 + b B2 + c C2) + K3 = 0

            b (B1 - B2 * A1 / A2) + c (C1 - C2 * A1 / A2) - K2 * A1 / A2 + K1 = 0
            b (B3 - B1 * A3 / A2) + c (C3 - C1 * A3 / A2) - K2 * A3 / A2 + K3  = 0
            **/
            (float b, float c) = solve2x2System(
                B1 - B2 * A1 / A2, C1 - C2 * A1 / A2, -K2 * A1 / A2 + K1,
                B3 - B1 * A3 / A2, C3 - C1 * A3 / A2, -K2 * A3 / A2 + K3
            );
            var a =  -(K2 + b * B2 + c * C2) / A2;
            return (a, b, c);
        }
        else if (A3 != 0)
        {
            /**
            (1) a = -(K3 + b B3 + c C3) / A3
            
            b B1 + c C1 - A1 / A3 * (K3 + b B3 + c C3) + K1 = 0
            b B2 + c C2 - A2 / A3 * (K3 + b B3 + c C3) + K2 = 0
            
            b (B1 - B3 A1 / A3) + c (C1 - C3 A1 / A3) - K3 * A1 / A3 + K1 = 0
            b (B2 - B3 A2 / A3) + c (C2 - C3 A2 / A3) - K3 * A2 / A3 + K2 = 0
            **/
            (float b, float c) = solve2x2System(
                B1 - B3 * A1 / A3, C1 - C3 * A1 / A3, -K3 * A1 / A3 + K1,
                B2 - B3 * A2 / A3, C2 - C3 * A2 / A3, -K3 * A2 / A3 + K2 
            );
            var a = -(K3 + b * B3 + c * C3) / A3;
            return (a, b, c);
        }
        else
        {
            (float b, float c) = solve2x2System(
                B1, C1, K1,
                B2, C2, K2
            );
            return (0, b, c);
        }
    }

    private static (float a, float b) solve2x2System(
        float A1, float B1, float K1,
        float A2, float B2, float K2
    )
    {

        float a = 0, b = 0;
        if (A1 == 0 && A2 == 0)
        {
            a = 0;
            b = B1 != 0 ?
                solve1x1System(B1, K1) :
                solve1x1System(B2, K2);
        }
        else if (B1 == 0 && B2 == 0)
        {
            a = A1 != 0 ?
                solve1x1System(A1, K1) :
                solve1x1System(A2, K2);
            b = 0;
        }
        else if (A1 != 0)
        {
            /**
            a A1 + b B1 + K1 = 0
            a A2 + b B2 + K2 = 0

            (1) a = (K1 - b B1) / A1

            A2 / A1 (K1 - b B1) + b B2 + K2 = 0
            (B2 - A2 / A1 B1) b + K2 + A2 / A1 K1 = 0
            b = -(K2 + A2 / A1 K1) / (B2 - A2 / A1 B1)
            **/
            b = -(K2 + A2 / A1 * K1) / (B2 - A2 / A1 * B1);
            a = (K1 - b * B1) / A1;
        }
        else if (A2 != 0)
        {
            /**
            a A1 + b B1 + K1 = 0
            a A2 + b B2 + K2 = 0

            (1) a = (K2 - b B2) / A2

            A1 / A2 (K2 - b B2) + b B1 + K1 = 0
            (B1 - A1 / A2 B2) b + K1 + A1 / A2 K2 = 0
            b = -(K1 + A1 / A2 K2) / (B1 - A1 / A2 B2)
            **/
            b = -(K1 + A1 / A2 * K2) / (B1 - A1 / A2 * B2);
            a = (K2 - b * B2) / A2;
        }
        else
        {
            a = 0;
            b = K1 / B1;
        }
        return (a, b);
    }

    private static float solve1x1System(float A, float B)
    {
        if (A == 0 || B == 0)
            return 0;
        
        return -B / A;
    }
}