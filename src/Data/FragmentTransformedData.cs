/* Author:  Leonardo Trevisan Silio
 * Date:    23/08/2023
 */
using System;
using System.Linq;
using System.Collections.Generic;

namespace Radiance.Data;

using ShaderSupport;
using ShaderSupport.Objects;

/// <summary>
/// Represents a transformed data
/// </summary>
public class FragmentTransformedData<D> : IData<D>
    where D : ShaderObject, new()
{
    IData<D> original;
    Func<D, Vec4ShaderObject> transformation;
    ShaderOutput<D> output;

    public FragmentTransformedData(
        IData<D> original, 
        Func<D, Vec4ShaderObject> transformation
    )
    {
        this.original = original;
        this.transformation = transformation;
        this.output = ShaderOutput.Create(original.Data1);
    }

    public event Action OnChange;
    public void HasChanged()
    {
        if (OnChange is null)
            return;
        
        OnChange();
    }

    public Vec3ShaderObject VertexObject => 
        original.VertexObject;

    public Vec4ShaderObject FragmentObject =>
        transformation(this.output.Dependence);

    public IEnumerable<ShaderOutput> Outputs => 
        original.Outputs
        .Append(this.output);

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
public class FragmentTransformedData<D1, D2> : IData<D1, D2>
    where D1 : ShaderObject, new()
    where D2 : ShaderObject, new()
{
    IData<D1, D2> original;
    Func<D1, D2, Vec4ShaderObject> transformation;
    ShaderOutput<D1> output1;
    ShaderOutput<D2> output2;

    public FragmentTransformedData(
        IData<D1, D2> original, 
        Func<D1, D2, Vec4ShaderObject> transformation
    )
    {
        this.original = original;
        this.transformation = transformation;
        this.output1 = ShaderOutput.Create(original.Data1);
        this.output2 = ShaderOutput.Create(original.Data2);
    }
    
    public event Action OnChange;
    public void HasChanged()
    {
        if (OnChange is null)
            return;
        
        OnChange();
    }

    public Vec3ShaderObject VertexObject => 
        original.VertexObject;

    public Vec4ShaderObject FragmentObject =>
        transformation(
            this.output1.Dependence, 
            this.output2.Dependence
        );

    public IEnumerable<ShaderOutput> Outputs => 
        original.Outputs
        .Append(this.output1)
        .Append(this.output2);

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