/* Author:  Leonardo Trevisan Silio
 * Date:    19/02/2024
 */
using System;
using System.Linq;
using System.Collections.Generic;

using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics.Arm;
using System.Numerics;

namespace Radiance.Internal;

/// <summary>
/// Contains operations to transform vectors data
/// </summary>
internal static class VectorsOperations
{
    private const int sortTreshold = 8;

    internal static (float a, float b, float c, float d) PlaneRegression(float[] pts)
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
        // monotone subdivision
        
        monotonePlaneTriangulation(orderMap, 
            points, triangules, 5
        );
        return triangules.ToArray();
    }

    private static void monotonePlaneTriangulation(
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

    private static int[] sort(float[] data, int size, int offsetA, int offsetB = -1)
    {
        var orderMap = new int[data.Length / size];
        for (int i = 0, n = 0; i < orderMap.Length; i++, n += size)
            orderMap[i] = n;

        quickSort(data, offsetA, offsetB, size, orderMap, 0, orderMap.Length);

        return orderMap;
    }

    private static void quickSort(
        float[] data, int offsetA, int offsetB, int size, 
        int[] map, int start, int end
    )
    {
        int len = end - start;
        if (len < sortTreshold)
        {
            slowSort(data, offsetA, offsetB, size, map, start, end);
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
            
            var temp = map[i];
            map[i] = map[j];
            map[j] = temp;
        }

        if (data[map[j] + offsetA] < pivo)
            j++;

        var oldPivo = map[j];
        map[j] = map[end - 1];
        map[end - 1] = oldPivo;

        quickSort(data, offsetA, offsetB, size, map, start, j);
        quickSort(data, offsetA, offsetB, size, map, j, end);
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
                    if (v1 <= v2)
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
}