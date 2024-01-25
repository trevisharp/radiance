/* Author:  Leonardo Trevisan Silio
 * Date:    24/01/2024
 */
using System.Collections.Generic; 

namespace Radiance.Shaders.Objects;

/// <summary>
/// Represent a Float data in shader implementation.
/// </summary>
public record FloatShaderObject : ShaderObject
{
    public FloatShaderObject(
        string value, ShaderOrigin origin,
        IEnumerable<ShaderDependence> deps
        ) : base(ShaderType.Bool, value, origin, deps) { }

    public static implicit operator FloatShaderObject(float value)
        => new (value.ToString().Replace(',', '.'), ShaderOrigin.Global, []);
        
    public static implicit operator FloatShaderObject(double value)
        => new (value.ToString().Replace(',', '.'), ShaderOrigin.Global, []);
        
    public static implicit operator FloatShaderObject(int value)
        => new (value.ToString(), ShaderOrigin.Global, []);
    
    public static BoolShaderObject operator ==(FloatShaderObject a, FloatShaderObject b)
        => Union<FloatShaderObject, FloatShaderObject, BoolShaderObject>(
            $"({a} == {b})", a, b);
    
    public static BoolShaderObject operator !=(FloatShaderObject a, FloatShaderObject b)
        => Union<FloatShaderObject, FloatShaderObject, BoolShaderObject>(
            $"({a} != {b})", a, b);

    public static BoolShaderObject operator <(FloatShaderObject a, FloatShaderObject b)
        => Union<FloatShaderObject, FloatShaderObject, BoolShaderObject>(
            $"({a} < {b})", a, b);

    public static BoolShaderObject operator >(FloatShaderObject a, FloatShaderObject b)
        => Union<FloatShaderObject, FloatShaderObject, BoolShaderObject>(
            $"({a} > {b})", a, b);
    
    public static BoolShaderObject operator <=(FloatShaderObject a, FloatShaderObject b)
        => Union<FloatShaderObject, FloatShaderObject, BoolShaderObject>(
            $"({a} <= {b})", a, b);

    public static BoolShaderObject operator >=(FloatShaderObject a, FloatShaderObject b)
        => Union<FloatShaderObject, FloatShaderObject, BoolShaderObject>(
            $"({a} >= {b})", a, b);

    public static FloatShaderObject operator +(FloatShaderObject a, FloatShaderObject b)
        => Union($"({a} + {b})", a, b);
    
    public static FloatShaderObject operator -(FloatShaderObject a, FloatShaderObject b)
        => Union($"({a} - {b})", a, b);
    
    public static FloatShaderObject operator *(FloatShaderObject a, FloatShaderObject b)
        => Union($"({a} * {b})", a, b);
    
    public static FloatShaderObject operator /(FloatShaderObject a, FloatShaderObject b)
        => Union($"({a} / {b})", a, b);

    public static FloatShaderObject operator +(FloatShaderObject a)
        => Transform<FloatShaderObject, FloatShaderObject>($"(+{a})", a);
    
    public static FloatShaderObject operator -(FloatShaderObject a)
        => Transform<FloatShaderObject, FloatShaderObject>($"(-{a})", a);
}