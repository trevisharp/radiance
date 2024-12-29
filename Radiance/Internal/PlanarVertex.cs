/* Author:  Leonardo Trevisan Silio
 * Date:    29/12/2024
 */
using System;

namespace Radiance.Internal;

/// <summary>
/// Represents a Planar Vertex.
/// </summary>
public readonly struct PlanarVertex(int id, float[] points, int index, float[] planePoints, int pindex)
{
    public readonly int Id = id;
    public readonly float X = points[index];
    public readonly float Y = points[index + 1];
    public readonly float Z = points[index + 2];
    public readonly float Xp = planePoints[pindex];
    public readonly float Yp = planePoints[pindex + 1];

    public static void ToPlanarVertex(float[] points, Span<PlanarVertex> vertices)
    {
        var planarPoints = ToPlanarPoints(points);

        for (int i = 0, j = 0, k = 0; k < vertices.Length; i += 3, j += 2, k++)
            vertices[k] = new PlanarVertex(k, points, i, planarPoints, j);
    }
    
    /// <summary>
    /// Transforma (x, y, z)[] array into a (x, y, z, x', y') array wheres
    /// x' and y' is the projection on a specific plane. 
    /// </summary>
    static float[] ToPlanarPoints(float[] original)
    {
        var plane = PlaneRegression(original);
        var result = new float[2 * original.Length / 3];

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

        for (int i1 = 0, i2 = 0; i1 < original.Length; i1 += 3, i2 += 2)
        {
            result[i2] = A * original[i1 + j] + B1;
            result[i2 + 1] = A * original[i1 + k] + B2;
        }
        
        return result;
    }

    /// <summary>
    /// Find a plane (ax + by + cz + d = 0) that better match the points.
    /// </summary>
    static (float a, float b, float c, float d) PlaneRegression(float[] pts)
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
}