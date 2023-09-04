/* Author:  Leonardo Trevisan Silio
 * Date:    03/09/2023
 */
using System.Collections.Generic;

namespace Radiance.Data;

using ShaderSupport;
using ShaderSupport.Objects;
using ShaderSupport.Dependencies;

/// <summary>
/// Represents a ARGB Color
/// </summary>
/// <param name="A">Transparence channel</param>
/// <param name="R">Red value</param>
/// <param name="G">Green value</param>
/// /// <param name="B">Blue value</param>
public class Color : BaseData<Vec4ShaderObject>
{
    public Color(float a, float r, float g, float b)
    {
        this.A = a;
        this.R = r;
        this.G = g;
        this.B = b;
    }

    public float A { get; set; }
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }

    public override Vec3ShaderObject VertexObject => (0, 0, 0);

    private ColorBufferDependence dep => new ColorBufferDependence(this);
    public override Vec4ShaderObject FragmentObject => dep;

    public override IEnumerable<ShaderOutput> Outputs
        => ShaderOutput.Empty;

    public override int Size => 4; 
    public override int Elements => 1;
    public override IEnumerable<int> Sizes => new int[] { 4 };

    public override Vec4ShaderObject Data1 => dep;

    public static readonly Color White = new Color(1, 1, 1, 1);
    public static readonly Color Black = new Color(1, 0, 0, 0);
    public static readonly Color Red = new Color(1, 1, 0, 0);
    public static readonly Color Green = new Color(1, 0, 1, 0);
    public static readonly Color Blue = new Color(1, 0, 0, 1);
    public static readonly Color Yellow = new Color(1, 1, 1, 0);
    public static readonly Color Magenta = new Color(1, 1, 0, 1);
    public static readonly Color Cyan = new Color(1, 0, 1, 1);

    public static implicit operator Vec4ShaderObject(Color color)
        => new Vec4ShaderObject(
            $"vec4({color.R}, {color.G}, {color.B}, {color.A})"
        );

    public override void SetData(float[] arr, ref int indexoff)
    {
        arr[indexoff + 0] = R;
        arr[indexoff + 1] = G;
        arr[indexoff + 2] = B;
        arr[indexoff + 3] = A;
        indexoff += 4;
    }
}