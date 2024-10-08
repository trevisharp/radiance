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
    private float[]? lastState = null;
    readonly List<float> data = [];
    public float Count => data.Count;
    
    public void Add(float value)
    {
        lastState = null;
        data.Add(value);
    }

    public void Clear()
    {
        lastState = null;
        data.Clear();
    }

    public float[] Data => lastState ??= [ ..data ];

    public int Vertices { get; set; }

    public Buffer? Buffer { get; set; }

    public IBufferedData Triangulation => this;
}