/* Author:  Leonardo Trevisan Silio
 * Date:    07/09/2023
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
    internal float[] Delaunay(float[] pts)
    {
        List<float> result = new List<float>();

        for (int i = 0; i < pts.Length - 6; i += 3)
        {
            var x1 = pts[i + 0];
            var y1 = pts[i + 1];
            var z1 = pts[i + 2];

            result.Add(x1);
            result.Add(y1);
            result.Add(z1);
            
            var x2 = pts[i + 3];
            var y2 = pts[i + 4];
            var z2 = pts[i + 5];

            var dx = x1 - x2;
            var dy = y1 - y2;
            var dz = z1 - z2;

            var d1 = dx * dx + dy * dy + dz * dz;
            var i1 = i + 3;

            x2 = pts[i + 6];
            y2 = pts[i + 7];
            z2 = pts[i + 8];

            dx = x1 - x2;
            dy = y1 - y2;
            dz = z1 - z2;

            var d2 = dx * dx + dy * dy + dz * dz;
            var i2 = i + 6;

            var j = 0;
            var dj = 0f;

            if (d2 < d1)
            {
                dj = d1;
                d1 = d2;
                d2 = dj;

                j = i1;
                i1 = i2;
                i2 = j;
            }

            for (j = i + 9; j < pts.Length; j += 3)
            {
                x2 = pts[j + 0];
                y2 = pts[j + 1];
                z2 = pts[j + 2];

                dx = x1 - x2;
                dy = y1 - y2;
                dz = z1 - z2;

                dj = dx * dx + dy * dy + dz * dz;

                if (dj < d1)
                {
                    d2 = d1;
                    i2 = i1;

                    d1 = dj;
                    i1 = j;
                }
                else if (dj < d2)
                {
                    d2 = dj;
                    i2 = j;
                }
            }

            result.Add(pts[i1 + 0]);
            result.Add(pts[i1 + 1]);
            result.Add(pts[i1 + 2]);

            result.Add(pts[i2 + 0]);
            result.Add(pts[i2 + 1]);
            result.Add(pts[i2 + 2]);
        }

        return result.ToArray();
    }
}