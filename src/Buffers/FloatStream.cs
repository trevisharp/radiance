/* Author:  Leonardo Trevisan Silio
 * Date:    06/11/2024
 */
using System;

namespace Radiance.Buffers;

public class FloatStream : IBufferedData
{
    int count = 0;
    float[] data = new float[10];
    Buffer? buffer = null;

    public int Rows => count;
    public int Columns => 1;
    public int Instances => count;
    public bool IsGeometry => false;
    public Buffer Buffer => buffer ??= Buffer.From(this);
    public int InstanceLength => 1;

    public float[] GetBufferData()
        => data[..count];

    /// <summary>
    /// Prepare the stream to recive data
    /// improving Add performance.
    /// </summary>
    public void PrepareSize(int size)
    {
        int space = data.Length - count;
        if (size < space)
            return;
        
        Expand(size);
    }

    /// <summary>
    /// Add a value on this data stream.
    /// </summary>
    public void Add(float value)
    {
        ExpandIfNeeded();
        data[count] = value;
        count++;
    }
    
    /// <summary>
    /// Clear this data stream.
    /// </summary>
    public void Clear()
    {
        data = new float[10];
        count = 0;
    }
    
    void ExpandIfNeeded()
    {
        if (count < data.Length)
            return;
        
        Expand(4 * data.Length);
    }

    void Expand(int expansion)
    {
        var newData = new float[expansion];
        Array.Copy(data, newData, data.Length);
        data = newData;
    }

    public static RepeatStream operator *(FloatStream stream, int times)
        => new(stream, times);
        
    public static RepeatStream operator *(int times, FloatStream stream)
        => new(stream, times);
}