/* Author:  Leonardo Trevisan Silio
 * Date:    28/08/2023
 */
#pragma warning disable CS0660
#pragma warning disable CS0661

using System.Reflection;

namespace Radiance.Shaders;

using Dependencies;

/// <summary>
/// A type to variables used in shaders
/// </summary>
public class ShaderGlobalReference
{
    public virtual object ObjectValue { get; }
}

/// <summary>
/// A type to variables used in shaders
/// </summary>
public class ShaderGlobalReference<T, S> : ShaderGlobalReference
    where S : ShaderObject, new()
{
    public override object ObjectValue => this.Value;

    public T Value { get; set; }
    public FieldInfo Field { get; set; }
    public object BaseObject { get; set; }

    public ShaderGlobalReference(
        FieldInfo field,
        object baseObject,
        object initialValue
    )
    {
        this.Field = field;
        this.BaseObject = baseObject;
        this.Value = (T)initialValue;
    }

    public static implicit operator S(ShaderGlobalReference<T, S> refObject)
        => new GlobalUniformDependence<S>(
            refObject.Field,
            refObject.BaseObject
        );

    public static implicit operator ShaderGlobalReference<T, S>(T value)
        => new (null, null, value);
    
    public static bool operator <(ShaderGlobalReference<T, S> x, T y)
    {
        dynamic dynX = x.Value;
        dynamic dynY = y;
        return dynX < dynY;
    }
    
    public static bool operator >(ShaderGlobalReference<T, S> x, T y)
    {
        dynamic dynX = x.Value;
        dynamic dynY = y;
        return dynX > dynY;
    }
    
    public static bool operator <=(ShaderGlobalReference<T, S> x, T y)
    {
        dynamic dynX = x.Value;
        dynamic dynY = y;
        return dynX <= dynY;
    }
    
    public static bool operator >=(ShaderGlobalReference<T, S> x, T y)
    {
        dynamic dynX = x.Value;
        dynamic dynY = y;
        return dynX >= dynY;
    }
    
    public static bool operator ==(ShaderGlobalReference<T, S> x, T y)
    {
        dynamic dynX = x.Value;
        dynamic dynY = y;
        return dynX == dynY;
    }
    
    public static bool operator !=(ShaderGlobalReference<T, S> x, T y)
    {
        dynamic dynX = x.Value;
        dynamic dynY = y;
        return dynX != dynY;
    }
}