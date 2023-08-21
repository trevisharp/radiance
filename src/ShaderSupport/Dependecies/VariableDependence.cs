/* Author:  Leonardo Trevisan Silio
 * Date:    21/08/2023
 */
namespace Radiance.ShaderSupport.Dependencies;

/// <summary>
/// Represents a dependece of a position buffer data.
/// </summary>
public class VariableDependence<T> : ShaderDependence<T>
    where T : ShaderObject, new()
{
    private static int count = 0;

    private string type;
    public VariableDependence()
    {
        count++;
        this.DependenceType = ShaderDependenceType.Variable;
        this.type = ShaderObject.GetStringName<T>();
        this.Name = $"param{count}";
    }

    public override object Value => null;

    public override string GetHeader()
        => $"in {type} {Name};";
}