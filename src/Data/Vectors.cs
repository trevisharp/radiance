/* Author:  Leonardo Trevisan Silio
 * Date:    05/09/2023
 */
using System.Collections.Generic;

namespace Radiance.Data;

using ShaderSupport;
using ShaderSupport.Objects;
using ShaderSupport.Dependencies;

/// <summary>
/// Represents a group of Vectors.
/// </summary>
public class Vectors : DataCollection<Vec3ShaderObject>
{   
    public void Add(Vector vec)
    {
        this.data.Add(vec.x);
        this.data.Add(vec.y);
        this.data.Add(vec.z);
        elements++;
    }

    public void Add(float x, float y, float z)
    {
        this.data.Add(x);
        this.data.Add(y);
        this.data.Add(z);
        elements++;
    }

    public override Vec3ShaderObject VertexObject => Data1;

    public override Vec4ShaderObject FragmentObject => Color.White;

    public override IEnumerable<ShaderOutput> Outputs => ShaderOutput.Empty;

    public override IEnumerable<int> Sizes => new int[] { 3 };
    public override Vec3ShaderObject Data1 => 
        new PositionBufferDependence(this);
}