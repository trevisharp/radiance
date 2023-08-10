/* Author:  Leonardo Trevisan Silio
 * Date:    10/08/2023
 */
using System.Linq;
using System.Collections.Generic;

namespace Radiance.ShaderSupport.Objects;

/// <summary>
/// Represent a Bool data in shader implementation.
/// </summary>
public class BoolShaderObject : ShaderObject
{
    public BoolShaderObject(string value, params ShaderObject[] dependecies)
    {
        this.Expression = value;
        this.Dependecies = dependecies;
        this.Type = ShaderType.Bool;
    }

    public BoolShaderObject(string value, IEnumerable<ShaderObject> dependecies)
    {
        this.Expression = value;
        this.Dependecies = dependecies;
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