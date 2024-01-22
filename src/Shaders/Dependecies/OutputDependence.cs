/* Author:  Leonardo Trevisan Silio
 * Date:    22/01/2024
 */
namespace Radiance.Shaders.Dependecies;

/// <summary>
/// Represents a input dependence from Vertex Shader to Fragment Shader.
/// </summary>
public class InputDependence<T> : ShaderDependence<T>
    where T : ShaderObject, new()
{
    private string type;
    public VariableDependence(string name)
    {
        this.DependenceType = ShaderDependenceType.Variable;
        this.type = ShaderObject.GetStringName<T>();
        this.Name = name;
    }

    public override object Value => null;
    public override string GetHeader()
        => $"out {type} {Name};";
}