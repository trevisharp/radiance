/* Author:  Leonardo Trevisan Silio
 * Date:    24/01/2024
 */
using System.Collections.Generic;
using System.Linq;

namespace Radiance.Shaders.Objects;

/// <summary>
/// Represent a Bool data in shader implementation.
/// </summary>
public record BoolShaderObject : ShaderObject
{
    public BoolShaderObject(
        string value, ShaderOrigin origin,
        IEnumerable<ShaderDependence> deps
        ) : base(ShaderType.Bool, value, origin, deps) { }

    public static BoolShaderObject operator &(BoolShaderObject a, BoolShaderObject b)
        => new ($"{a} && {b}", a.Dependecies.Concat(b.Dependecies));

    public static BoolShaderObject operator |(BoolShaderObject a, BoolShaderObject b)
        => new ($"{a.Expression} || {b.Expression}", a.Dependecies.Concat(b.Dependecies));
    
    public static BoolShaderObject operator !(BoolShaderObject a)
        => new ($"!({a.Expression})", a.Dependecies);

    public static implicit operator BoolShaderObject(bool value)
        => new (value.ToString(), ShaderOrigin.Global, []);
}