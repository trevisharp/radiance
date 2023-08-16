/* Author:  Leonardo Trevisan Silio
 * Date:    15/08/2023
 */
namespace Radiance.ShaderSupport.Dependencies;

using Objects;
using Data;

/// <summary>
/// Represents a dependence of a input color
/// </summary>
public class ColorDependee : ShaderDependence<Vec4ShaderObject>
{
    private Color color;

    public ColorDependee(Color color)
    {
        this.color = color;
        this.Name = "color";
        this.DependenceType = ShaderDependenceType.Uniform;
    }

    public override object Value
        => this.color;

    public override string GetHeader()
        => "uniform vec4 color;";
}