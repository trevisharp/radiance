/* Author:  Leonardo Trevisan Silio
 * Date:    21/01/2024
 */
namespace Radiance.Shaders.Dependencies;

using Objects;

/// <summary>
/// Represent a parameter in a function used to create a render.
/// </summary>
public class UniformParameterDependence : ShaderDependence<FloatShaderObject>
{
    private string type;
    private object value;
    public override object Value => value;

    public UniformParameterDependence(string name, string type)
    {
        this.DependenceType = ShaderDependenceType.Uniform;
        this.Name = name;
        this.type = type;
    }

    public void SetValue(object value)
        => this.value = value;

    public override string GetHeader()
        => $"uniform {type} {Name};";
}