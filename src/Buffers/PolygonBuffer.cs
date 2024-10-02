/* Author:  Leonardo Trevisan Silio
 * Date:    02/10/2024
 */
using System;

namespace Radiance.Buffers;

/// <summary>
/// Represents a complex polygon with many data on vertices.
/// </summary>
public class PolygonBuffer : IBufferedData
{
    public PolygonBuffer(Polygon basePolygon)
    {
        var triangulation = basePolygon.Triangulation.Data;
        triangules = new float[triangulation.Length];
        Array.Copy(triangulation, triangules, triangules.Length);
    }

    float[] buffer = [];
    float[] triangules;
    private int polygonCount = 0;
    private int size = 3;

    public float[] Data => buffer;

    public int Vertices => polygonCount;

    public Buffer? Buffer { get; set; }

    public IBufferedData Triangulation => this;
}