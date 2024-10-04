/* Author:  Leonardo Trevisan Silio
 * Date:    03/10/2024
 */
using System.Collections.Generic;

namespace Radiance.Buffers;

/// <summary>
/// Represents a simple buffer with many data on vertices.
/// </summary>
public class SimpleBuffer : IBufferedData
{
    readonly List<float> data = [];
    public float Count => data.Count;
    
    public void Add(float value)
        => data.Add(value);

    public void Clear()
        => data.Clear();

    public float[] Data => [ ..data ];

    public int Vertices { get; set; }

    public Buffer? Buffer { get; set; }

    public IBufferedData Triangulation => this;
}