/* Author:  Leonardo Trevisan Silio
 * Date:    11/10/2024
 */
using System;

namespace Radiance.Buffers;

/// <summary>
/// A buffered data with a polygon repeated many times.
/// </summary>
public class RepeatPolygon(Polygon polygon, int times) : IBufferedData
{
    float[] Repeat(float[] data)
    {
        var len = data.Length;
        var buffer = new float[len * times];

        for (int i = 0; i < times; i++)
            Array.Copy(data, 0, buffer, len * i, len);
        
        return buffer;
    }

    float[] BuildData()
        => Repeat(polygon.Data);

    float[] BuildTriangules()
        => Repeat(polygon.Triangules.Data);

    TrianguleBuffer? triangulationPair = null;
    public TrianguleBuffer Triangules
    {
        get
        {
            triangulationPair ??= new(BuildTriangules(), 3);
            return triangulationPair;
        }
    }

    float[]? data = null;
    public float[] Data
    {
        get
        {
            data ??= BuildData();
            return data;
        }
    }

    public int Vertices => polygon.Vertices * times;

    public Buffer? Buffer { get; set; }
}