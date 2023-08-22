/* Author:  Leonardo Trevisan Silio
 * Date:    22/08/2023
 */
using System.Reflection;

namespace Radiance.ShaderSupport;

using Dependencies;

/// <summary>
/// A type to global variables used in shaders
/// </summary>
public class GlobalShaderObject : ShaderObject
{ 
    protected FieldInfo field;
    protected object baseObject;
    
    public object Value { get; set; }

    public GlobalShaderObject(FieldInfo field, object baseObject)
    {
        this.field = field;
        this.baseObject = baseObject;
    
    }

    public GlobalShaderObject(object value)
    {
        this.Value = value;
    }
}

/// <summary>
/// A type to global variables used in shaders
/// </summary>
public class GlobalShaderObject<T, S> : GlobalShaderObject
    where S : ShaderObject, new()
{
    public GlobalShaderObject(FieldInfo field, object baseObject)
        : base (field, baseObject) { }

    public GlobalShaderObject(object value)
        : base (value) { }

    public static implicit operator S(GlobalShaderObject<T, S> globalObject)
    {
        if (globalObject.field is not null && globalObject.baseObject is not null)
            return new GlobalUniformDependence<S>(
                globalObject.field, globalObject.baseObject
            );

        var obj =  new S();
        obj.Expression = globalObject.Value.ToString();
        return obj;
    }
}