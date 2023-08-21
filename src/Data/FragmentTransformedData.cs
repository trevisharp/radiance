/* Author:  Leonardo Trevisan Silio
 * Date:    21/08/2023
 */
using System;
using System.Collections.Generic;

namespace Radiance.Data;

using ShaderSupport;

/// <summary>
/// Represents a transformed data
/// </summary>
public class FragmentTransformedData<V, F1, F2> : IData<V, F2>
    where V : ShaderObject, new()
    where F1 : ShaderObject, new()
    where F2 : ShaderObject, new()
{
    IData<V, F1> original;
    Func<F1, F2> transformation;
    public FragmentTransformedData(IData<V, F1> original, Func<F1, F2> transformation)
    {
        this.original = original;
        this.transformation = transformation;
    }

    public V VertexObject => original.VertexObject;

    public F2 FragmentObject => 
        transformation(original.FragmentObject);

    public IEnumerable<ShaderOutput> Outputs => original.Outputs;

    public int Size => original.Size;
    public int Elements => original.Elements;
    public IEnumerable<int> Sizes => original.Sizes;

    public float[] GetBuffer()
        => original.GetBuffer();

    public void SetData(float[] arr, ref int indexoff)
        => original.SetData(arr, ref indexoff);
}