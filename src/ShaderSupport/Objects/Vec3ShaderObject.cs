/* Author:  Leonardo Trevisan Silio
 * Date:    10/08/2023
 */
using System;
using System.Collections.Generic;

namespace Radiance.ShaderSupport.Objects;

/// <summary>
/// Represent a Vec3 data in shader implementation.
/// </summary>
public class Vec3ShaderObject : ShaderObject
{
    public Vec3ShaderObject(string value, params ShaderObject[] dependecies)
    {
        this.Expression = value;
        this.Dependecies = dependecies;
        this.Type = ShaderType.Vec3;
    }

    public Vec3ShaderObject(string value, IEnumerable<ShaderObject> dependecies)
    {
        this.Expression = value;
        this.Dependecies = dependecies;
        this.Type = ShaderType.Vec3;
    }

    public FloatShaderObject this[int index]
    {
        get
        {
            if (index < 0 || index > 2)
                throw new Exception("Um vec3 só pode ser acesso dos indidices 0 a 2");

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

    public static implicit operator Vec3ShaderObject((float x, float y, float z) tuple)
        => new Vec3ShaderObject($"vec3({tuple.x}, {tuple.y}, {tuple.z})");
}