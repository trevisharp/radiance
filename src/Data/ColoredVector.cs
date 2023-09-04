/* Author:  Leonardo Trevisan Silio
 * Date:    03/09/2023
 */
using System.Collections.Generic;

namespace Radiance.Data;

using ShaderSupport;
using ShaderSupport.Objects;
using ShaderSupport.Dependencies;

public class ColoredVector : BaseData<Vec3ShaderObject, Vec4ShaderObject>
{
    public Vector Vector { get; set; }
    public Color Color { get; set; }

    public override Vec3ShaderObject VertexObject => Data1;
    public override Vec4ShaderObject FragmentObject => Data2;
    public override IEnumerable<ShaderOutput> Outputs 
        => ShaderOutput.Empty;

    public override int Size => 7;
    public override int Elements => 1;
    public override IEnumerable<int> Sizes => new int[] { 3, 4 };

    public override Vec3ShaderObject Data1
        => new PositionBufferDependence(this, 0);
    
    public override Vec4ShaderObject Data2
        => new ColorBufferDependence(this, 1);

    public override void SetData(float[] arr, ref int indexoff)
    {
        this.Vector.SetData(arr, ref indexoff);
        this.Color.SetData(arr, ref indexoff);
    }
}