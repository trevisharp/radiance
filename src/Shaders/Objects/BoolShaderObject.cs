/* Author:  Leonardo Trevisan Silio
 * Date:    10/08/2023
 */
using System.Linq;
using System.Collections.Generic;

namespace Radiance.Shaders.Objects;

/// <summary>
/// Represent a Bool data in shader implementation.
/// </summary>
public record BoolShaderObject : ShaderObject
{
    public BoolShaderObject() 
        : base(ShaderType.Bool, "false", ShaderOrigin.Global, [])
    { }

    public BoolShaderObject(string value, params ShaderDependence[] deps)
    {
        this.Expression = value;
        this.Dependecies = deps;
        this.Type = ShaderType.Bool;
    }

    public BoolShaderObject(string value, IEnumerable<ShaderDependence> deps)
    {
        this.Expression = value;
        this.Dependecies = deps;
        this.Type = ShaderType.Bool;
    }

    public static BoolShaderObject operator &(BoolShaderObject a, BoolShaderObject b)
        => new BoolShaderObject($"{a.Expression} && {b.Expression}", a.Dependecies.Concat(b.Dependecies));

    public static BoolShaderObject operator |(BoolShaderObject a, BoolShaderObject b)
        => new BoolShaderObject($"{a.Expression} || {b.Expression}", a.Dependecies.Concat(b.Dependecies));
    
    public static BoolShaderObject operator !(BoolShaderObject a)
        => new BoolShaderObject($"!({a.Expression})", a.Dependecies);

    public static implicit operator BoolShaderObject(bool value)
        => new BoolShaderObject($"{value}");
}