/* Author:  Leonardo Trevisan Silio
 * Date:    03/09/2023
 */
using System;
using System.Collections.Generic;

namespace Radiance.Data;

using ShaderSupport;
using ShaderSupport.Objects;
using ShaderSupport.Dependencies;

/// <summary>
/// Represents a group of Colored Vectors.
/// </summary>
public class ColoredVectors : BaseData<Vec3ShaderObject, Vec4ShaderObject>
{
    private int elements = 0;
    private List<float> vectors = new List<float>();

    public float this[int index]
    {
        get => vectors[index];
        set => vectors[index] = value;
    }
    
    public void Add(ColoredVector vec)
    {
        this.vectors.Add(vec.Vector.x);
        this.vectors.Add(vec.Vector.y);
        this.vectors.Add(vec.Vector.z);
        this.vectors.Add(vec.Color.R);
        this.vectors.Add(vec.Color.G);
        this.vectors.Add(vec.Color.B);
        this.vectors.Add(vec.Color.A);
        elements++;
    }

    public void Add(float x, float y, float z, float r, float g, float b, float a)
    {
        this.vectors.Add(x);
        this.vectors.Add(y);
        this.vectors.Add(z);
        this.vectors.Add(r);
        this.vectors.Add(g);
        this.vectors.Add(b);
        this.vectors.Add(a);
        elements++;
    }

    public override Vec3ShaderObject VertexObject => Data1;
    public override Vec4ShaderObject FragmentObject => Data2;
    
    public override void SetData(float[] arr, ref int indexoff)
    {
        var copy = vectors.ToArray();
        Array.Copy(copy, 0, arr, indexoff, copy.Length);
        indexoff += copy.Length;
    }

    public override int Size => this.vectors.Count;
    public override int Elements => this.elements;
    public override IEnumerable<ShaderOutput> Outputs => ShaderOutput.Empty;

    public override IEnumerable<int> Sizes => new int[] { 3, 4 };
    public override Vec3ShaderObject Data1 
        => new PositionBufferDependence(this, 0);
    public override Vec4ShaderObject Data2
        => new ColorBufferDependence(this, 1);
}