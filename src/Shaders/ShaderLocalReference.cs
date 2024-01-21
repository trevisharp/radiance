/* Author:  Leonardo Trevisan Silio
 * Date:    28/08/2023
 */
#pragma warning disable CS0660
#pragma warning disable CS0661

namespace Radiance.Shaders;

using Internal;
using Dependencies;

/// <summary>
/// A type to variables used in shaders
/// </summary>
public class ShaderLocalReference<T, S>
    where S : ShaderObject, new()
{
    public string Name { get; set; }
    public T Value { get; set; }

    public ShaderLocalReference(
        string name,
        T value
    )
    {
        this.Name = name;
        this.Value = value;
    }

    public void Update(T value)
        => this.Value = value;

    public static implicit operator S(ShaderLocalReference<T, S> refObject)
        => new LocalUniformDependence<S>(
                refObject.Name,
                refObject.Value
        );

    public static implicit operator ShaderLocalReference<T, S>(T value)
    {
        var paramName = ParamNamgeGenerator.GetNext();
        return new (paramName, value);
    }
    
    public static bool operator <(ShaderLocalReference<T, S> x, T y)
    {
        dynamic dynX = x.Value;
        dynamic dynY = y;
        return dynX < dynY;
    }
    
    public static bool operator >(ShaderLocalReference<T, S> x, T y)
    {
        dynamic dynX = x.Value;
        dynamic dynY = y;
        return dynX > dynY;
    }
    
    public static bool operator <=(ShaderLocalReference<T, S> x, T y)
    {
        dynamic dynX = x.Value;
        dynamic dynY = y;
        return dynX <= dynY;
    }
    
    public static bool operator >=(ShaderLocalReference<T, S> x, T y)
    {
        dynamic dynX = x.Value;
        dynamic dynY = y;
        return dynX >= dynY;
    }
    
    public static bool operator ==(ShaderLocalReference<T, S> x, T y)
    {
        dynamic dynX = x.Value;
        dynamic dynY = y;
        return dynX == dynY;
    }
    
    public static bool operator !=(ShaderLocalReference<T, S> x, T y)
    {
        dynamic dynX = x.Value;
        dynamic dynY = y;
        return dynX != dynY;
    }
}