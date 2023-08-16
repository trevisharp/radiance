/* Author:  Leonardo Trevisan Silio
 * Date:    15/08/2023
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
    public override int SetData(float[] arr, int indexoff)
        => original.SetData(arr, indexoff);
}