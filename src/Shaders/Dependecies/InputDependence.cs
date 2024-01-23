/* Author:  Leonardo Trevisan Silio
 * Date:    23/01/2024
 */
namespace Radiance.Shaders.Dependencies;

/// <summary>
/// Represents a input dependence from Vertex Shader to Fragment Shader.
/// </summary>
public class InputDependence : ShaderDependence
{
    private string type;
    public InputDependence(string name, string type)
    {
        this.DependenceType = ShaderDependenceType.Variable;
        this.type = type;
        this.Name = name;
    }

    public override object Value => null;
    public override string GetHeader()
        => $"in {type} {Name};";
}