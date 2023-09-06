/* Author:  Leonardo Trevisan Silio
 * Date:    05/09/2023
 */
using System.Collections.Generic;

namespace Radiance.Data;

using ShaderSupport.Objects;
using ShaderSupport.Dependencies;

/// <summary>
/// Represents a group of Colored Vectors.
/// </summary>
public class ColoredVectors : DataCollection<Vec3ShaderObject, Vec4ShaderObject>
{
    public void Add(ColoredVector vec)
    {
        this.data.Add(vec.Vector.x);
        this.data.Add(vec.Vector.y);
        this.data.Add(vec.Vector.z);
        this.data.Add(vec.Color.R);
        this.data.Add(vec.Color.G);
        this.data.Add(vec.Color.B);
        this.data.Add(vec.Color.A);
        elements++;
    }

    public void Add(float x, float y, float z, float r, float g, float b, float a)
    {
        this.data.Add(x);
        this.data.Add(y);
        this.data.Add(z);
        this.data.Add(r);
        this.data.Add(g);
        this.data.Add(b);
        this.data.Add(a);
        elements++;
    }

    public override Vec3ShaderObject VertexObject => Data1;
    public override Vec4ShaderObject FragmentObject => Data2;

    public override IEnumerable<int> Sizes => new int[] { 3, 4 };
    public override Vec3ShaderObject Data1 
        => new PositionBufferDependence(this, 0);
    public override Vec4ShaderObject Data2
        => new ColorBufferDependence(this, 1);
}