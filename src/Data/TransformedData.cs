/* Author:  Leonardo Trevisan Silio
 * Date:    18/08/2023
 */
using System;

namespace Radiance.Data;

using Radiance.ShaderSupport.Objects;
using ShaderSupport;

/// <summary>
/// Represents a transformed data
/// </summary>
public class TransformedData<D, T> : Data<D, T>
    where D : ShaderDependence<T>
    where T : ShaderObject, new()
{
    private Data<D, T> original;
    private Func<T, T> transformation;

    public TransformedData(
        Data<D, T> original,
        Func<T, T> transformation
    )
    {
        this.original = original;
        this.transformation = transformation;
    }

    public override T ToObject => transformation(original.ToObject);
    public override Vec4ShaderObject DataColor => original.DataColor;
    public override int Size => original.Size;
    public override int Elements => original.Elements;
    public override D ToDependence => original.ToDependence;
    public override void SetData(float[] arr, ref int indexoff)
        => original.SetData(arr, ref indexoff);
}

public class TransformedData<D1, D2, T1, T2> : Data<D1, D2, T1, T2>
    where D1 : ShaderDependence<T1>
    where D2 : ShaderDependence<T2>
    where T1 : ShaderObject, new()
    where T2 : ShaderObject, new()
{
    private Data<D1, D2, T1, T2> original;
    private Func<T1, T2, (T1, T2)> transformation;

    public TransformedData(
        Data<D1, D2, T1, T2> original,
        Func<T1, T2, (T1, T2)> transformation
    )
    {
        this.original = original;
        this.transformation = transformation;
    }

    private (T1, T2)? temp = null;

    public override T1 ToObject1 => (
            temp ??= transformation(
            original.ToObject1,
            original.ToObject2
            )
        ).Item1;

    public override T2 ToObject2 => (
            temp ??= transformation(
            original.ToObject1,
            original.ToObject2
            )
        ).Item2;

    public override Vec4ShaderObject DataColor => original.DataColor;
    public override int Size1 => original.Size1;
    public override int Size2 => original.Size2;
    public override int Elements => original.Elements;
    public override D1 ToDependence1 => original.ToDependence1;
    public override D2 ToDependence2 => original.ToDependence2;
    public override void SetData(float[] arr, ref int indexoff)
        => original.SetData(arr, ref indexoff);
}