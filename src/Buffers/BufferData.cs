/* Author:  Leonardo Trevisan Silio
 * Date:    04/12/2024
 */
using System;

namespace Radiance.Buffers;

/// <summary>
/// Represents a Data used on a buffer.
/// </summary>
public class BufferData(int size, int instanceLen, bool isGeometry) : IBufferedData
{
    int count = 0;
    float[] data = new float[10];
    Buffer? buffer = null;
    
    public int Rows => count / size;

    public int Columns => size;

    public int Instances => Rows / instanceLen;

    public int InstanceLength => instanceLen;

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
    /// Add a value on this data stream.
    /// </summary>
    public void AddRange(float[] value)
    {
        ExpandIfNeeded(count + value.Length);
        Array.Copy(value, 0, data, count, value.Length);
        count += value.Length;
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
        while (finalSize < expectedSize)
            finalSize *= 4;
        
        Expand(finalSize);
    }

    void Expand(int size)
    {
        var newData = new float[size];
        Array.Copy(data, newData, data.Length);
        data = newData;
    }

    public static VirtualBufferData operator *(BufferData stream, int times)
        => new(stream, times);
        
    public static VirtualBufferData operator *(int times, BufferData stream)
        => new(stream, times);
}