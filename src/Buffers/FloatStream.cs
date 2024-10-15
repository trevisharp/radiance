/* Author:  Leonardo Trevisan Silio
 * Date:    14/10/2024
 */
using System;

namespace Radiance.Buffers;

public class FloatStream : IMutableData
{
    int count = 0;
    float[] data = new float[10];

    public int Count => count;

    public int Size => 1;

    public Buffer? Buffer { get; set; }

    public void PrepareSize(int size)
    {
        int space = data.Length - count;
        if (size < space)
            return;
        
        Expand(size);
    }

    public void Add(float value)
    {
        ExpandIfNeeded();
        data[count] = value;
        count++;
    }
    
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
}