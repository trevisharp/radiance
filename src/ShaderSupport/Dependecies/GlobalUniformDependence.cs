/* Author:  Leonardo Trevisan Silio
 * Date:    21/08/2023
 */
using System.Reflection;

namespace Radiance.ShaderSupport.Dependencies;

/// <summary>
/// Represents a dependece of a position buffer data.
/// </summary>
public class GlobalVariableDependence<T> : ShaderDependence<T>
    where T : ShaderObject, new()
{
    private string type;
    private FieldInfo field;
    private object baseType;
    public GlobalVariableDependence(FieldInfo field, object baseType)
    {
        this.DependenceType = ShaderDependenceType.Uniform;
        this.type = ShaderObject.GetStringName<T>();
        this.Name = field.Name;
        this.baseType = baseType;
        this.field = field;
    }

    public override object Value
        => field.GetValue(baseType);

    public override string GetHeader()
        => $"uniform {type} {Name};";
}