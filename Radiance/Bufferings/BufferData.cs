/* Author:  Leonardo Trevisan Silio
 * Date:    04/12/2024
 */
using System;

namespace Radiance.Bufferings;

/// <summary>
/// Represents a Data used on a buffer.
/// </summary>
public class BufferData(int rowSize, int instanceLen, bool isGeometry) : IBufferedData
{
    int valueCount = 0;
    float[] data = new float[10];
    Buffer? buffer = null;
    
    public int Rows => valueCount / rowSize;

    public int Columns => rowSize;

    public int Instances => Rows / instanceLen;

    public int InstanceLength => instanceLen;

    public bool IsGeometry => isGeometry;

    public Buffer Buffer => buffer ??= Buffer.From(this);

    public float[] GetBufferData()
        => data[..valueCount];
    
    /// <summary>
    /// Prepare the stream to recive data
    /// improving Add performance.
    /// </summary>
    public void PrepareSize(int size)
    {
        int space = data.Length - valueCount;
        if (size < space)
            return;
        
        Expand(valueCount + size);
    }

    /// <summary>
    /// Add a value on this data stream.
    /// </summary>
    public void Add<T>(T value)
        where T : IBufferizable
    {
        int size = value.ComputeSize();
        ExpandIfNeeded(valueCount + size);
        value.Bufferize(data, valueCount);
        valueCount += size;
    }

    /// <summary>
    /// Add a value on this data stream.
    /// </summary>
    public void Add(float value)
    {
        ExpandIfNeeded(valueCount + 1);
        data[valueCount] = value;
        valueCount++;
    }
    
    /// <summary>
    /// Add a value on this data stream.
    /// </summary>
    public void AddRange(float[] value)
    {
        ExpandIfNeeded(valueCount + value.Length);
        Array.Copy(value, 0, data, valueCount, value.Length);
        valueCount += value.Length;
    }

    /// <summary>
    /// Clear this data stream.
    /// </summary>
    public void Clear()
    {
        data = new float[10];
        valueCount = 0;
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

    public override string ToString()
        => $$"""
        BufferData {
            Rows: {{Rows}},
            Columns: {{Columns}},
            Instances: {{Instances}},
            InstanceLength: {{InstanceLength}},
            IsGeometry: {{IsGeometry}}
        }
        """;
}