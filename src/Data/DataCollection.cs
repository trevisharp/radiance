/* Author:  Leonardo Trevisan Silio
 * Date:    05/09/2023
 */
using System;
using System.Collections.Generic;

namespace Radiance.Data;

using Radiance.ShaderSupport;
using Radiance.ShaderSupport.Objects;
using ShaderSupport;
using ShaderSupport.Objects;

/// <summary>
/// A base class for easy implements all IData has Collections
/// </summary>
public abstract class DataCollection : BaseData
{
    protected int elements = 0;
    protected List<float> vectors = new List<float>();

    public float this[int index]
    {
        get => vectors[index];
        set => vectors[index] = value;
    }
    
    public override int Size => this.vectors.Count;
    public override int Elements => elements;
    public override IEnumerable<ShaderOutput> Outputs => ShaderOutput.Empty;

    public override void SetData(float[] arr, ref int indexoff)
    {
        var copy = vectors.ToArray();
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
        var copy = data.ToArray();
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
        var copy = data.ToArray();
        Array.Copy(copy, 0, arr, indexoff, copy.Length);
        indexoff += copy.Length;
    }
}