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