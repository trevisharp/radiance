/* Author:  Leonardo Trevisan Silio
 * Date:    17/08/2023
 */
namespace Radiance.Data;

using ShaderSupport;
using ShaderSupport.Objects;

/// <summary>
/// A base class to all data layouts.
/// </summary>
public interface IData
{
    int Size { get; }
    int Elements { get; }
    int SetData(float[] arr, int indexoff);
    float[] GetBuffer()
    {
        float[] buffer = new float[this.Size];

        this.SetData(buffer, 0);
        
        return buffer;
    }
    Vec4ShaderObject DataColor { get; }
}

/// <summary>
/// A base class to all data layouts with one dependence.
/// </summary>
public abstract class Data<D, T> : IData
    where D : ShaderDependence<T>
    where T : ShaderObject, new()
{
    public abstract int Size { get; }
    public abstract int Elements { get; }

    public abstract D ToDependence { get; }
    public virtual T ToObject => ToDependence;

    public virtual Vec4ShaderObject DataColor => Color.White;
    public abstract int SetData(float[] arr, int indexoff);
    
    public static implicit operator D(Data<D, T> data)
        => data.ToDependence;

    public static implicit operator T(Data<D, T> data)
        => data.ToObject;
}

/// <summary>
/// A base class to all data layouts with two dependence.
/// </summary>
public abstract class Data<D1, D2, T1, T2> : IData
    where D1 : ShaderDependence<T1>
    where D2 : ShaderDependence<T2>
    where T1 : ShaderObject, new()
    where T2 : ShaderObject, new()
{
    public int Size => this.Size1 + this.Size2;
    public abstract int Elements { get; }
    
    public abstract int Size1 { get; }
    public abstract D1 ToDependence1 { get; }
    public virtual T1 ToObject1 => ToDependence1;

    public abstract int Size2 { get; }
    public abstract D2 ToDependence2 { get; }
    public virtual T2 ToObject2 => ToDependence2;

    public virtual Vec4ShaderObject DataColor => Color.White;
    public abstract int SetData(float[] arr, int indexoff);


    public static implicit operator D1(Data<D1, D2, T1, T2> data)
        => data.ToDependence1;
    
    public static implicit operator D2(Data<D1, D2, T1, T2> data)
        => data.ToDependence2;

    public static implicit operator T1(Data<D1, D2, T1, T2> data)
        => data.ToObject1;
    
    public static implicit operator T2(Data<D1, D2, T1, T2> data)
        => data.ToObject2;
}