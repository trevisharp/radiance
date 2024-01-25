/* Author:  Leonardo Trevisan Silio
 * Date:    24/01/2024
 */
using System.Collections.Generic;

namespace Radiance.Shaders.Objects;

/// <summary>
/// Represent a Bool data in shader implementation.
/// </summary>
public class BoolShaderObject : ShaderObject
{
    public BoolShaderObject(
        string value, ShaderOrigin origin,
        IEnumerable<ShaderDependence> deps
        ) : base(ShaderType.Bool, value, origin, deps) { }

    public static BoolShaderObject operator &(BoolShaderObject a, BoolShaderObject b)
        => Union($"({a} & {b})", a, b);

    public static BoolShaderObject operator |(BoolShaderObject a, BoolShaderObject b)
        => Union($"({a} || {b})", a, b);
    
    public static BoolShaderObject operator !(BoolShaderObject a)
        => Transform<BoolShaderObject, BoolShaderObject>($"(!{a})", a);

    public static implicit operator BoolShaderObject(bool value)
        => new (value.ToString(), ShaderOrigin.Global, []);
}