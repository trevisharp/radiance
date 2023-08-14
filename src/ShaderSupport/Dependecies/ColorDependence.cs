/* Author:  Leonardo Trevisan Silio
 * Date:    14/08/2023
 */
namespace Radiance.ShaderSupport.Dependencies;

using Data;

/// <summary>
/// Represents a dependence of a input color
/// </summary>
public class ColorDependee : ShaderDependence
{
    private Color color;

    public ColorDependee(Color color)
    {
        this.color = color;
        this.Name = "color";
        this.Type = ShaderType.Vec4;
        this.DependenceType = ShaderDependenceType.Uniform;
    }

    public override object Value
        => this.color;

    public override string GetHeader(params object[] args)
        => "uniform vec4 color";
}