/* Author:  Leonardo Trevisan Silio
 * Date:    21/08/2023
 */
using System;
using System.Collections.Generic;

namespace Radiance.Data;

using ShaderSupport;
using ShaderSupport.Objects;

/// <summary>
/// Represents a transformed data
/// </summary>
public class VertexTransformedData<D> : IData<D>
    where D : ShaderObject, new()
{
    IData<D> original;
    Func<D, Vec3ShaderObject> transformation;
    public VertexTransformedData(
        IData<D> original,
        Func<D, Vec3ShaderObject> transformation
    )
    {
        this.original = original;
        this.transformation = transformation;
    }

    public Vec3ShaderObject VertexObject =>
        transformation(original.Data1);

    public Vec4ShaderObject FragmentObject => 
        original.FragmentObject;

    public IEnumerable<ShaderOutput> Outputs => 
        original.Outputs;

    public int Size => original.Size;
    public int Elements => original.Elements;
    public IEnumerable<int> Sizes => original.Sizes;

    public D Data1 => original.Data1;

    public float[] GetBuffer()
        => original.GetBuffer();

    public void SetData(float[] arr, ref int indexoff)
        => original.SetData(arr, ref indexoff);
}


/// <summary>
/// Represents a transformed data
/// </summary>
public class VertexTransformedData<D1, D2> : IData<D1, D2>
    where D1 : ShaderObject, new()
    where D2 : ShaderObject, new()
{
    IData<D1, D2> original;
    Func<D1, D2, Vec3ShaderObject> transformation;
    public VertexTransformedData(
        IData<D1, D2> original,
        Func<D1, D2, Vec3ShaderObject> transformation
    )
    {
        this.original = original;
        this.transformation = transformation;
    }

    public Vec3ShaderObject VertexObject =>
        transformation(original.Data1, original.Data2);

    public Vec4ShaderObject FragmentObject => 
        original.FragmentObject;

    public IEnumerable<ShaderOutput> Outputs => 
        original.Outputs;

    public int Size => original.Size;
    public int Elements => original.Elements;
    public IEnumerable<int> Sizes => original.Sizes;

    public D1 Data1 => original.Data1;

    public D2 Data2 => original.Data2;

    public float[] GetBuffer()
        => original.GetBuffer();

    public void SetData(float[] arr, ref int indexoff)
        => original.SetData(arr, ref indexoff);
}