/* Author:  Leonardo Trevisan Silio
 * Date:    10/08/2023
 */
using System;
using System.Linq;
using System.Collections.Generic;

namespace Radiance.ShaderSupport.Objects;

/// <summary>
/// Represent a Vec3 data in shader implementation.
/// </summary>
public class Vec3ShaderObject : ShaderObject
{
    public Vec3ShaderObject()
    {
        this.Expression = "(0.0, 0.0, 0.0)";
        this.Dependecies = new ShaderDependence[0];
        this.Type = ShaderType.Vec3;
    }

    public Vec3ShaderObject(string value, params ShaderDependence[] dependecies)
    {
        this.Expression = value;
        this.Dependecies = dependecies;
        this.Type = ShaderType.Vec3;
    }

    public Vec3ShaderObject(string value, IEnumerable<ShaderDependence> dependecies)
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

    public static BoolShaderObject operator ==(Vec3ShaderObject a, Vec3ShaderObject b)
    {
        return new BoolShaderObject(
            $"({a.Expression}) == ({b.Expression})",
            a.Dependecies.Concat(b.Dependecies)
        );
    }
    
    public static BoolShaderObject operator !=(Vec3ShaderObject a, Vec3ShaderObject b)
    {
        return new BoolShaderObject(
            $"({a.Expression}) != ({b.Expression})",
            a.Dependecies.Concat(b.Dependecies)
        );
    }

    public static implicit operator Vec3ShaderObject((float x, float y, float z) tuple)
        => new Vec3ShaderObject($"vec3({tuple.x}, {tuple.y}, {tuple.z})");

    public static implicit operator Vec3ShaderObject((FloatShaderObject x, FloatShaderObject y, FloatShaderObject z) tuple)
        => new Vec3ShaderObject(
            $"vec3({tuple.x.Expression}, {tuple.y.Expression}, {tuple.z.Expression})",
            tuple.x.Dependecies.Concat(tuple.y.Dependecies).Concat(tuple.z.Dependecies)
        );
        
    public static implicit operator Vec3ShaderObject((Vec2ShaderObject xy, FloatShaderObject z) tuple)
        => new Vec3ShaderObject(
            $"vec3({tuple.xy.Expression}, {tuple.z.Expression})",
            tuple.xy.Dependecies.Concat(tuple.z.Dependecies)
        );
        
    public static implicit operator Vec3ShaderObject((FloatShaderObject x, Vec2ShaderObject yz) tuple)
        => new Vec3ShaderObject(
            $"vec3({tuple.x.Expression}, {tuple.yz.Expression})",
            tuple.x.Dependecies.Concat(tuple.yz.Dependecies)
        );
}