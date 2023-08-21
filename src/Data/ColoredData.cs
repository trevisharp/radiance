/* Author:  Leonardo Trevisan Silio
 * Date:    21/08/2023
 */
using System;

namespace Radiance.Data;

using ShaderSupport;
using ShaderSupport.Objects;

/// <summary>
/// Represents a transformed data
/// </summary>
public class ColoredData<D, T> : Data<D, T>
    where D : ShaderDependence<T>
    where T : ShaderObject, new()
{
    private Data<D, T> original;
    private Func<Vec4ShaderObject> transformation;

    public ColoredData(
        Data<D, T> original,
        Func<Vec4ShaderObject> transformation
    )
    {
        this.original = original;
        this.transformation = transformation;
    }

    public override Vec4ShaderObject DataColor => transformation();
    public override T ToObject => original.ToObject;
    public override int Size => original.Size;
    public override int Elements => original.Elements;
    public override D ToDependence => original.ToDependence;
    public override void SetData(float[] arr, ref int indexoff)
        => original.SetData(arr, ref indexoff);
}


public class ColoredData<D1, D2, T1, T2> : Data<D1, T1>
    where D1 : ShaderDependence<T1>
    where D2 : ShaderDependence<T2>
    where T1 : ShaderObject, new()
    where T2 : ShaderObject, new()
{
    private Data<D1, D2, T1, T2> original;
    private Func<T2, Vec4ShaderObject> transformation;

    public ColoredData(
        Data<D1, D2, T1, T2> original,
        Func<T2, Vec4ShaderObject> transformation
    )
    {
        this.original = original;
        this.transformation = transformation;
    }

    public override Vec4ShaderObject DataColor
        => transformation(original.ToObject2);
    public override T1 ToObject => original.ToObject1;

    public override int Size => original.Size;
    public override int Elements => original.Elements;
    
    public override D1 ToDependence => original.ToDependence1;
    
    public override void SetData(float[] arr, ref int indexoff)
        => original.SetData(arr, ref indexoff);
}