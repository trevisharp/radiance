/* Author:  Leonardo Trevisan Silio
 * Date:    10/08/2023
 */
#pragma warning disable CS0660
#pragma warning disable CS0661

using System;
using System.Linq;
using System.Collections.Generic;

namespace Radiance.Shaders.Objects;

using Internal;

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

    public static Vec3ShaderObject operator +(Vec3ShaderObject v, Vec3ShaderObject u)
        => new Vec3ShaderObject(
            $"({v}) + ({u})",
            v.Dependecies.Concat(u.Dependecies)
        );
    
    public static Vec3ShaderObject operator -(Vec3ShaderObject v, Vec3ShaderObject u)
        => new Vec3ShaderObject(
            $"({v}) - ({u})",
            v.Dependecies.Concat(u.Dependecies)
        );
    
    public static FloatShaderObject operator *(Vec3ShaderObject v, Vec3ShaderObject u)
        => new FloatShaderObject(
            $"({v}) * ({u})",
            v.Dependecies.Concat(u.Dependecies)
        );
    
    public static Vec3ShaderObject operator *(Vec3ShaderObject v, FloatShaderObject a)
    {
        var dependecies = v.Dependecies.Concat(a.Dependecies);
        return new ($"({a}) * ({v})", dependecies);
    }

    public static Vec3ShaderObject operator *(FloatShaderObject a, Vec3ShaderObject v)
    {
        var dependecies = v.Dependecies.Concat(a.Dependecies);
        return new ($"({a}) * ({v})", dependecies);
    }

    public static Vec3ShaderObject operator /(Vec3ShaderObject v, FloatShaderObject a)
    {
        var dependecies = v.Dependecies.Concat(a.Dependecies);
        return new ($"({v}) / ({a})", dependecies);
    }
    
    public static Vec3ShaderObject operator +(Vec3ShaderObject v, 
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z) tuple)
    {
        Vec3ShaderObject u = tuple;
        return v + u;
    }

    public static Vec3ShaderObject operator +(
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z) tuple, 
        Vec3ShaderObject v)
    {
        Vec3ShaderObject u = tuple;
        return v + u;
    }
    
    public static Vec3ShaderObject operator -(Vec3ShaderObject v,
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z) tuple)
    {
        Vec3ShaderObject u = tuple;
        return v - u;
    }

    public static Vec3ShaderObject operator -(
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z) tuple, 
        Vec3ShaderObject v)
    {
        Vec3ShaderObject u = tuple;
        return v - u;
    }
    
    public static FloatShaderObject operator *(Vec3ShaderObject v, 
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z) tuple)
    {
        Vec3ShaderObject u = tuple;
        return v * u;
    }

    public static FloatShaderObject operator *(
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z) tuple,
        Vec3ShaderObject v)
    {
        Vec3ShaderObject u = tuple;
        return v * u;
    }

    public static implicit operator Vec3ShaderObject((float x, float y, float z) tuple)
        => new Vec3ShaderObject($"vec3({tuple.x.Format()}, {tuple.y.Format()}, {tuple.z.Format()})");

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
    
    public static Vec3ShaderObject operator +(Vec3ShaderObject x)
    {
        var dependecies = x.Dependecies;
        return new ($"+({x})", dependecies);
    }
    
    public static Vec3ShaderObject operator -(Vec3ShaderObject x)
    {
        var dependecies = x.Dependecies;
        return new ($"-({x})", dependecies);
    }
}