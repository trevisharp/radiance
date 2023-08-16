/* Author:  Leonardo Trevisan Silio
 * Date:    15/08/2023
 */
namespace Radiance.ShaderSupport.Dependencies;

using Objects;

/// <summary>
/// Represents a input for the width of
/// screen used in shader implementation.
/// </summary>
public class WidthWindowDependence : ShaderDependence<FloatShaderObject>
{
    public WidthWindowDependence()
    {
        this.Name = "width";
        this.DependenceType = ShaderDependenceType.Uniform;
    }

    public override object Value
        => (float)Window.Width;

    public override string GetHeader()
        => "uniform float width;";
}