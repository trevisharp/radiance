/* Author:  Leonardo Trevisan Silio
 * Date:    10/08/2023
 */
using System;
using System.Linq;
using System.Collections.Generic;

namespace Radiance.ShaderSupport.Objects;

/// <summary>
/// Represent a Vec2 data in shader implementation.
/// </summary>
public class Vec2ShaderObject : ShaderObject
{
    public Vec2ShaderObject(string value, params ShaderObject[] dependecies)
    {
        this.Expression = value;
        this.Dependecies = dependecies;
        this.Type = ShaderType.Vec2;
    }

    public Vec2ShaderObject(string value, IEnumerable<ShaderObject> dependecies)
    {
        this.Expression = value;
        this.Dependecies = dependecies;
        this.Type = ShaderType.Vec2;
    }

    public FloatShaderObject this[int index]
    {
        get
        {
            if (index < 0 || index > 1)
                throw new Exception("Um Vec2 só pode ser acesso dos indidices 0 a 1");

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
    
    public static BoolShaderObject operator ==(Vec2ShaderObject a, Vec2ShaderObject b)
    {
        return new BoolShaderObject(
            $"({a.Expression}) == ({b.Expression})",
            a.Dependecies.Concat(b.Dependecies)
        );
    }
    
    public static BoolShaderObject operator !=(Vec2ShaderObject a, Vec2ShaderObject b)
    {
        return new BoolShaderObject(
            $"({a.Expression}) != ({b.Expression})",
            a.Dependecies.Concat(b.Dependecies)
        );
    }

    public static implicit operator Vec2ShaderObject((float x, float y) tuple)
        => new Vec2ShaderObject($"Vec2({tuple.x}, {tuple.y})");
}