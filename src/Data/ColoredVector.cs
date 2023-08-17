/* Author:  Leonardo Trevisan Silio
 * Date:    17/08/2023
 */
namespace Radiance.Data;

using ShaderSupport.Dependencies;
using ShaderSupport.Objects;

public class ColoredVector : Data
    <PositionBufferDependence,
    ColorBufferDependence,
    Vec3ShaderObject,
    Vec3ShaderObject>
{
    public Vector Vector { get; set; }
    public Color Color { get; set; }

    public override int Elements => 1;

    public override int Size1 => 3;
    public override PositionBufferDependence ToDependence1
        => new PositionBufferDependence(this.GetBuffer(), 0);

    public override int Size2 => 3;
    public override ColorBufferDependence ToDependence2
        => new ColorBufferDependence(this.GetBuffer(), 1);

    public override void SetData(float[] arr, ref int indexoff)
    {
        this.Vector.SetData(arr, ref indexoff);
        arr[indexoff + 0] = Color.R / 255f;
        arr[indexoff + 1] = Color.G / 255f;
        arr[indexoff + 2] = Color.B / 255f;
        indexoff += 3;
    }
}