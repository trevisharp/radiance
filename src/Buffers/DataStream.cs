/* Author:  Leonardo Trevisan Silio
 * Date:    04/12/2024
 */
using System;

namespace Radiance.Buffers;

public class DataStream(int size, bool isGeometry) : IBufferedData
{
    int count = 0;
    float[] data = new float[10];
    Buffer? buffer = null;
    
    public int Rows => count;

    public int Columns => size;

    public int Instances => count;

    public int InstanceLength => 1;

    public bool IsGeometry => isGeometry;

    public Buffer Buffer => buffer ??= Buffer.From(this);

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
        
        Expand(count + size);
    }

    /// <summary>
    /// Add a value on this data stream.
    /// </summary>
    public void Add<T>(T value)
        where T : IBufferizable
    {
        int size = value.ComputeSize();
        ExpandIfNeeded(count + size);
        value.Bufferize(data, count);
        count += size;
    }

    /// <summary>
    /// Add a value on this data stream.
    /// </summary>
    public void Add(float value)
    {
        ExpandIfNeeded(count + 1);
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
    
    void ExpandIfNeeded(int expectedSize)
    {
        if (expectedSize < data.Length)
            return;

        int finalSize = data.Length;
        while (expectedSize < finalSize)
            finalSize *= 4;
        
        Expand(finalSize);
    }

    void Expand(int size)
    {
        var newData = new float[size];
        Array.Copy(data, newData, data.Length);
        data = newData;
    }


    public static RepeatStream operator *(DataStream stream, int times)
        => new(stream, times);
        
    public static RepeatStream operator *(int times, DataStream stream)
        => new(stream, times);
}