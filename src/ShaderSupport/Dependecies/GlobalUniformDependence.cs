/* Author:  Leonardo Trevisan Silio
 * Date:    28/08/2023
 */
using System.Reflection;

namespace Radiance.ShaderSupport.Dependencies;

/// <summary>
/// Represents a dependece of a position buffer data.
/// </summary>
public class GlobalUniformDependence<T> : ShaderDependence<T>
    where T : ShaderObject, new()
{
    private string type;
    private FieldInfo field;
    private object baseType;
    
    public GlobalUniformDependence(FieldInfo field, object baseType)
    {
        this.DependenceType = ShaderDependenceType.Uniform;
        this.type = ShaderObject.GetStringName<T>();
        this.Name = field.Name;
        this.baseType = baseType;
        this.field = field;
    }

    public override object Value
    {
        get
        {
            var globalObject = field.GetValue(baseType) as ShaderGlobalReference;
            return globalObject.ObjectValue;
        }
    }

    public override string GetHeader()
        => $"uniform {type} {Name};";
}