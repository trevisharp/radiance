/* Author:  Leonardo Trevisan Silio
 * Date:    15/08/2023
 */
namespace Radiance.Shaders.Dependencies;

using Objects;

/// <summary>
/// Represents a input for the width of
/// screen used in shader implementation.
/// </summary>
public class HeightWindowDependence : ShaderDependence<FloatShaderObject>
{
    public HeightWindowDependence()
    {
        this.Name = "height";
        this.DependenceType = ShaderDependenceType.Uniform;
    }

    public override object Value
        => (float)Window.Height;

    public override string GetHeader()
        => "uniform float height;";
}