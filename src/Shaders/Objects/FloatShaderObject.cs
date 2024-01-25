/* Author:  Leonardo Trevisan Silio
 * Date:    24/01/2024
 */
#pragma warning disable CS0660
#pragma warning disable CS0661

using System.Collections.Generic;
using Radiance.Internal;

namespace Radiance.Shaders.Objects;

/// <summary>
/// Represent a Float data in shader implementation.
/// </summary>
public class FloatShaderObject : ShaderObject
{
    public FloatShaderObject(
        string value, ShaderOrigin origin,
        IEnumerable<ShaderDependence> deps
        ) : base(ShaderType.Bool, value, origin, deps) { }

    public static implicit operator FloatShaderObject(float value)
        => new (value.Format(), ShaderOrigin.Global, []);
        
    public static implicit operator FloatShaderObject(double value)
        => new (value.Format(), ShaderOrigin.Global, []);
        
    public static implicit operator FloatShaderObject(int value)
        => new (value.Format(), ShaderOrigin.Global, []);
    
    public static BoolShaderObject operator ==(FloatShaderObject a, FloatShaderObject b)
        => Union<BoolShaderObject>($"({a} == {b})", a, b);
    
    public static BoolShaderObject operator !=(FloatShaderObject a, FloatShaderObject b)
        => Union<BoolShaderObject>($"({a} != {b})", a, b);

    public static BoolShaderObject operator <(FloatShaderObject a, FloatShaderObject b)
        => Union<BoolShaderObject>($"({a} < {b})", a, b);

    public static BoolShaderObject operator >(FloatShaderObject a, FloatShaderObject b)
        => Union<BoolShaderObject>($"({a} > {b})", a, b);
    
    public static BoolShaderObject operator <=(FloatShaderObject a, FloatShaderObject b)
        => Union<BoolShaderObject>($"({a} <= {b})", a, b);

    public static BoolShaderObject operator >=(FloatShaderObject a, FloatShaderObject b)
        => Union<BoolShaderObject>($"({a} >= {b})", a, b);

    public static FloatShaderObject operator +(FloatShaderObject a, FloatShaderObject b)
        => Union<FloatShaderObject>($"({a} + {b})", a, b);
    
    public static FloatShaderObject operator -(FloatShaderObject a, FloatShaderObject b)
        => Union<FloatShaderObject>($"({a} - {b})", a, b);
    
    public static FloatShaderObject operator *(FloatShaderObject a, FloatShaderObject b)
        => Union<FloatShaderObject>($"({a} * {b})", a, b);
    
    public static FloatShaderObject operator /(FloatShaderObject a, FloatShaderObject b)
        => Union<FloatShaderObject>($"({a} / {b})", a, b);

    public static FloatShaderObject operator +(FloatShaderObject a)
        => Transform<FloatShaderObject, FloatShaderObject>($"(+{a})", a);
    
    public static FloatShaderObject operator -(FloatShaderObject a)
        => Transform<FloatShaderObject, FloatShaderObject>($"(-{a})", a);
}