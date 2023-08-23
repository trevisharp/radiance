/* Author:  Leonardo Trevisan Silio
 * Date:    23/08/2023
 */
using System.Collections.Generic;

namespace Radiance.Data;

using ShaderSupport;
using ShaderSupport.Objects;
using ShaderSupport.Dependencies;

public class ColoredVector : IData<Vec3ShaderObject, Vec4ShaderObject>
{
    public Vector Vector { get; set; }
    public Color Color { get; set; }

    public Vec3ShaderObject VertexObject => Data1;
    public Vec4ShaderObject FragmentObject => Data2;

    public IEnumerable<ShaderOutput> Outputs 
        => ShaderOutput.Empty;

    public int Size => 7;
    public int Elements => 1;
    public IEnumerable<int> Sizes => new int[] { 3, 4 };

    public Vec3ShaderObject Data1
        => new PositionBufferDependence(this, 0);
    
    public Vec4ShaderObject Data2
        => new ColorBufferDependence(this, 1);

    public float[] GetBuffer()
    {
        float[] buffer = new float[this.Size];

        int indexoff = 0;
        this.SetData(buffer, ref indexoff);
        
        return buffer;
    }

    public void SetData(float[] arr, ref int indexoff)
    {
        this.Vector.SetData(arr, ref indexoff);
        this.Color.SetData(arr, ref indexoff);
    }
}