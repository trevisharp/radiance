/* Author:  Leonardo Trevisan Silio
 * Date:    10/08/2023
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace Radiance.ShaderSupport.Objects;

/// <summary>
/// Represent a Vec4 data in shader implementation.
/// </summary>
public class Vec4ShaderObject : ShaderObject
{
    public Vec4ShaderObject(string value, params ShaderObject[] dependecies)
    {
        this.Expression = value;
        this.Dependecies = dependecies;
        this.Type = ShaderType.Vec4;
    }

    public Vec4ShaderObject(string value, IEnumerable<ShaderObject> dependecies)
    {
        this.Expression = value;
        this.Dependecies = dependecies;
        this.Type = ShaderType.Vec4;
    }

    public FloatShaderObject this[int index]
    {
        get
        {
            if (index < 0 || index > 3)
                throw new Exception("Um vec4 só pode ser acesso dos indidices 0 a 3");

            return new FloatShaderObject(
                $"({Expression})[{index}]",
                this.Dependecies
            );
        }
    }

    public FloatShaderObject x
    {
        get => new FloatShaderObject(
            $"({Expression}).x",
            this.Dependecies
        );
    }
    
    public FloatShaderObject y
    {
        get => new FloatShaderObject(
            $"({Expression}).y",
            this.Dependecies
        );
    }
    
    public FloatShaderObject z
    {
        get => new FloatShaderObject(
            $"({Expression}).z",
            this.Dependecies
        );
    }

    public FloatShaderObject w
    {
        get => new FloatShaderObject(
            $"({Expression}).w",
            this.Dependecies
        );
    }
    
    public static BoolShaderObject operator ==(Vec4ShaderObject a, Vec4ShaderObject b)
    {
        return new BoolShaderObject(
            $"({a.Expression}) == ({b.Expression})",
            a.Dependecies.Concat(b.Dependecies)
        );
    }
    
    public static BoolShaderObject operator !=(Vec4ShaderObject a, Vec4ShaderObject b)
    {
        return new BoolShaderObject(
            $"({a.Expression}) != ({b.Expression})",
            a.Dependecies.Concat(b.Dependecies)
        );
    }

    public static implicit operator Vec4ShaderObject((float x, float y, float z, float w) tuple)
        => new Vec4ShaderObject($"vec3({tuple.x}, {tuple.y}, {tuple.z}, {tuple.w})");
}