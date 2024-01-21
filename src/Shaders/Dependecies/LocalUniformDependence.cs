/* Author:  Leonardo Trevisan Silio
 * Date:    28/08/2023
 */
namespace Radiance.Shaders.Dependencies;

/// <summary>
/// Represents a dependece of a position buffer data.
/// </summary>
public class LocalUniformDependence<T> : ShaderDependence<T>
    where T : ShaderObject, new()
{
    private string type;
    private object value;
    
    public LocalUniformDependence(string name, object value)
    {
        this.DependenceType = ShaderDependenceType.Uniform;
        this.type = ShaderObject.GetStringName<T>();
        this.Name = name;
        this.value = value;
    }

    public override object Value => value;

    public override string GetHeader()
        => $"uniform {type} {Name};";
}