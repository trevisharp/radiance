/* Author:  Leonardo Trevisan Silio
 * Date:    20/09/2023
 */
using System;
using System.Collections.Generic;

using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics.Arm;

namespace Radiance.Internal;

/// <summary>
/// Contains operations to transform vectors data
/// </summary>
internal class VectorsOperations
{
    internal float[] ConvexHull(float[] points)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Version 1.0 of triangularization.
    /// </summary>
    internal float[] PlanarDelaunay(float[] pts)
    {
        if (pts.Length < 9)
            return new float[0];
        
        var plane = PlaneRegression(pts);
        var transformed = transform(pts, plane);
        return delaunay(transformed);
    }
    
    private DelaunayPoint[] transform(float[] original, (float a, float b, float c, float d) plane)
    {
        throw new NotImplementedException();
    }

    private float[] delaunay(DelaunayPoint[] pts)
    {
        throw new NotImplementedException();
    }

    private struct DelaunayPoint
    {
        public float x;
        public float y;
        public float z;

        public float tx;
        public float ty;
    }

    internal (float a, float b, float c, float d) PlaneRegression(float[] pts)
    {
        float a, b, c, d;
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
          + 2ab pxy + 2ac pxz + 2ad xm
          + 2bc pyz + 2bd ym + 2cd zm
          + d^2

          qa = S a_i^2 / N
          pab = S a_i b_i / N
          am = S a_i / N
        **/

        var qx = 0f, qy = 0f, qz = 0f,
            pxy = 0f, pyz = 0f, pxz = 0f,
            xm = 0f, ym = 0f, zm = 0f;
        
        for (int i = 0; i < pts.Length; i += 3)
        {
            var x = pts[i + 0],
                y = pts[i + 1],
                z = pts[i + 2];
            
            qx += x * x;
            qy += y * y;
            qz += z * z;
            pxy += x * y;
            pyz += y * z;
            pxz += x * z;
            xm += x;
            xy += y;
            xz += z;
        }
        qx /= N;
        qy /= N;
        qz /= N;
        pxy /= N;
        pyz /= N;
        pxz /= N;
        xm /= N;
        xy /= N;
        xz /= N;
        
        /**
        d = 1

        dE/da = 2a qx + 2b pxy + 2c pxz + 2 xm
        a qx + b pxy + c pxz + xm = 0

        dE/db = 2b qy + 2a pxy + 2c pyz + 2 ym
        b qy + a pxy + c pyz + ym = 0

        dE/dc = 2c qz + 2a pxz + 2b pyz + 2 zm
        c qz + a pxz + b pyz + zm = 0

        a qx  + b pxy + c pxz + xm = 0
        a pxy + b qy  + c pyz + ym = 0
        a pxz + b pyz + c qz  + zm = 0
        **/
        
        d = 1f;
        (a, b, c) = solve3x3System(
            qx,  pxy, pxz, xm
            pxy, qy,  pyz, ym
            pxz, pyz, qz,  zm
        );

        return (a, b, c, d);
    }

    private (float a, float b, float c) solve3x3System(
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

    private (float a, float b) solve2x2System(
        float A1, float B1, float K1,
        float A2, float B2, float K2
    )
    {
        float a = 0, b = 0;
        if (A1 != 0)
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
            return (a, b);
        }
        return (a, b);
    }
}