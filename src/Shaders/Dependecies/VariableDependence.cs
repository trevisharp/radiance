/* Author:  Leonardo Trevisan Silio
 * Date:    21/08/2023
 */
namespace Radiance.Shaders.Dependencies;

using Internal;

/// <summary>
/// Represents a dependece of a position buffer data.
/// </summary>
public class VariableDependence<T> : ShaderDependence<T>
    where T : ShaderObject, new()
{
    private string type;
    public VariableDependence()
    {
        this.DependenceType = ShaderDependenceType.Variable;
        this.type = ShaderObject.GetStringName<T>();
        this.Name = ParamNamgeGenerator.GetNext();
    }

    public override object Value => null;

    public override string GetHeader()
        => $"in {type} {Name};";
}