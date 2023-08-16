using System;

namespace Radiance.Data;

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

    public override T ToObject
        => transformation(original.ToObject);

    public override int Size => original.Size;
    public override int Elements => original.Elements;
    public override D ToDependence => original.ToDependence;
    public override int SetData(float[] arr, int indexoff)
        => original.SetData(arr, indexoff);
}