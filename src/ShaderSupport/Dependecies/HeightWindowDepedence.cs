/* Author:  Leonardo Trevisan Silio
 * Date:    10/08/2023
 */
namespace Radiance.ShaderSupport.Dependencies;

/// <summary>
/// Represents a input for the width of
/// screen used in shader implementation.
/// </summary>
public class HeightWindowDependence : ShaderDependence
{
    public HeightWindowDependence()
    {
        this.Name = "height";
        this.Type = ShaderType.Float;
        this.DependenceType = ShaderDependenceType.Uniform;
    }

    public override object Value
        => (float)Window.Height;

    public override string GetHeader(params object[] args)
        => "uniform float height;";
}