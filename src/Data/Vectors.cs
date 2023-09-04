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
/// Represents a group of Vectors.
/// </summary>
public class Vectors : BaseData<Vec3ShaderObject>
{
    private int elements = 0;
    private List<float> vectors = new List<float>();
    
    public void Add(Vector vec)
    {
        this.vectors.Add(vec.x);
        this.vectors.Add(vec.y);
        this.vectors.Add(vec.z);
        elements++;
    }

    public void Add(float x, float y, float z)
    {
        this.vectors.Add(x);
        this.vectors.Add(y);
        this.vectors.Add(z);
        elements++;
    }

    private PositionBufferDependence dep =>
        new PositionBufferDependence(this);

    public override Vec3ShaderObject VertexObject => dep;

    public override Vec4ShaderObject FragmentObject => Color.White;
    
    public override void SetData(float[] arr, ref int indexoff)
    {
        var copy = vectors.ToArray();
        Array.Copy(copy, 0, arr, indexoff, copy.Length);
        indexoff += copy.Length;
    }

    public override int Size => this.vectors.Count;
    
    public override int Elements => this.elements;

    public override IEnumerable<ShaderOutput> Outputs => ShaderOutput.Empty;

    public override IEnumerable<int> Sizes => new int[] { 3 };
    public override Vec3ShaderObject Data1 => dep;
}