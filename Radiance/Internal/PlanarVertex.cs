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

    /// <summary>
    /// Load points transforming data on PLanarVertex points.
    /// </summary>
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
        var result = new float[2 * original.Length / 3];

        for (int i = 0, j = 0; i < original.Length; i += 3, j += 2)
        {
            result[j] = original[i];
            result[j + 1] = original[i + 1];
        }
        
        return result;
    }
}