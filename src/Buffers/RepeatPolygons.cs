/* Author:  Leonardo Trevisan Silio
 * Date:    14/10/2024
 */
using System;
using System.Collections.ObjectModel;

namespace Radiance.Buffers;

/// <summary>
/// A buffered data with a polygon repeated many times.
/// </summary>
public class RepeatPolygon(IPolygon polygon, int times) : IPolygon
{
    public int Count => polygon.Count * times;

    public int Size => polygon.Size;

    public Buffer? Buffer { get; set; }

    TrianguleBuffer? triangulationPair = null;
    public TrianguleBuffer Triangules
        => triangulationPair ??= new(BuildTriangules(), 3);

    ReadOnlyCollection<float>? data = null;
    public ReadOnlyCollection<float> Data
        => data ??= Array.AsReadOnly(BuildData());

    float[] BuildData()
        => Repeat(polygon.Data, times);

    float[] BuildTriangules()
        => Repeat(polygon.Triangules.Data, times);
    
    static float[] Repeat(ReadOnlyCollection<float> data, int times)
    {
        var len = data.Count;
        var buffer = new float[len * times];

        for (int i = 0; i < times; i++)
            data.CopyTo(buffer, len * i);
        
        return buffer;
    }
}