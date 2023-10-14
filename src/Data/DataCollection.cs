/* Author:  Leonardo Trevisan Silio
 * Date:    05/09/2023
 */
using System;
using System.Collections.Generic;

namespace Radiance.Data;

using Radiance.ShaderSupport;

/// <summary>
/// A base class for easy implements all IData has Collections
/// </summary>
public abstract class DataCollection : BaseData
{
    public float[] Buffer => data.ToArray();

    protected int elements = 0;
    protected List<float> data = new List<float>();

    public float this[int index]
    {
        get => data[index];
        set => data[index] = value;
    }
    
    public override int Size => this.data.Count;
    public override int Elements => elements;
    public override IEnumerable<ShaderOutput> Outputs => ShaderOutput.Empty;

    public override void SetData(float[] arr, ref int indexoff)
    {
        var copy = Buffer;
        Array.Copy(copy, 0, arr, indexoff, copy.Length);
        indexoff += copy.Length;
    }
}

/// <summary>
/// A base class for easy implements all IData has Collections
/// </summary>
public abstract class DataCollection<D> : BaseData<D>
    where D : ShaderObject, new()
{
    public float[] Buffer => data.ToArray();

    protected int elements = 0;
    protected List<float> data = new List<float>();

    public float this[int index]
    {
        get => data[index];
        set => data[index] = value;
    }
    
    public override int Size => this.data.Count;
    public override int Elements => elements;
    public override IEnumerable<ShaderOutput> Outputs => ShaderOutput.Empty;

    public override void SetData(float[] arr, ref int indexoff)
    {
        var copy = Buffer;
        Array.Copy(copy, 0, arr, indexoff, copy.Length);
        indexoff += copy.Length;
    }
}

/// <summary>
/// A base class for easy implements all IData has Collections
/// </summary>
public abstract class DataCollection<D1, D2> : BaseData<D1, D2>
    where D1 : ShaderObject, new()
    where D2 : ShaderObject, new()
{
    public float[] Buffer => data.ToArray();

    protected int elements = 0;
    protected List<float> data = new List<float>();

    public float this[int index]
    {
        get => data[index];
        set => data[index] = value;
    }
    
    public override int Size => this.data.Count;
    public override int Elements => elements;
    public override IEnumerable<ShaderOutput> Outputs => ShaderOutput.Empty;

    public override void SetData(float[] arr, ref int indexoff)
    {
        var copy = Buffer;
        Array.Copy(copy, 0, arr, indexoff, copy.Length);
        indexoff += copy.Length;
    }
}