/* Author:  Leonardo Trevisan Silio
 * Date:    03/09/2023
 */
using System;
using System.Collections.Generic;

namespace Radiance.Data;

using ShaderSupport;
using ShaderSupport.Objects;

/// <summary>
/// A base class for easy implements all IData
/// </summary>
public abstract class BaseData : IData
{
    public abstract Vec3ShaderObject VertexObject { get; }
    public abstract Vec4ShaderObject FragmentObject { get; }
    public abstract IEnumerable<ShaderOutput> Outputs { get; }
    public abstract int Size { get; }
    public abstract int Elements { get; }
    public abstract IEnumerable<int> Sizes { get; }

    public abstract void SetData(float[] arr, ref int indexoff);

    public float[] GetBuffer()
    {
        float[] buffer = new float[this.Size];

        int indexoff = 0;
        this.SetData(buffer, ref indexoff);
        
        return buffer;
    }

    public virtual void HasChanged()
    {
        if (OnChange is null)
            return;
        
        OnChange();
    }
    
    public event Action OnChange;
}

/// <summary>
/// A base class for easy implements all IData
/// </summary>
public abstract class BaseData<D> : BaseData, IData<D>
    where D : ShaderObject, new()
{
    public virtual D Data1 { get; }
}

/// <summary>
/// A base class for easy implements all IData
/// </summary>
public abstract class BaseData<D1, D2> : BaseData, IData<D1, D2>
    where D1 : ShaderObject, new()
    where D2 : ShaderObject, new()
{
    public virtual D1 Data1 { get; }
    public virtual D2 Data2 { get; }
}