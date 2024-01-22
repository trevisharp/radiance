/* Author:  Leonardo Trevisan Silio
 * Date:    10/08/2023
 */
#pragma warning disable CS0660
#pragma warning disable CS0661

using System;
using System.Collections.Generic;
using System.Linq;

namespace Radiance.Shaders.Objects;

using Internal;

/// <summary>
/// Represent a Vec4 data in shader implementation.
/// </summary>
public class Vec4ShaderObject : ShaderObject
{
    public Vec4ShaderObject()
    {
        this.Expression = "(0.0, 0.0, 0.0, 0.0)";
        this.Dependecies = new ShaderDependence[0];
        this.Type = ShaderType.Vec4;
    }

    public Vec4ShaderObject(string value, params ShaderDependence[] dependecies)
    {
        this.Expression = value;
        this.Dependecies = dependecies;
        this.Type = ShaderType.Vec4;
    }

    public Vec4ShaderObject(string value, IEnumerable<ShaderDependence> dependecies)
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
            $"({Expression}.x)",
            this.Dependecies
        );
    }
    
    public FloatShaderObject y
    {
        get => new FloatShaderObject(
            $"({Expression}.y)",
            this.Dependecies
        );
    }
    
    public FloatShaderObject z
    {
        get => new FloatShaderObject(
            $"({Expression}.z)",
            this.Dependecies
        );
    }

    public FloatShaderObject w
    {
        get => new FloatShaderObject(
            $"({Expression}.w)",
            this.Dependecies
        );
    }
    
    public static BoolShaderObject operator ==(Vec4ShaderObject a, Vec4ShaderObject b)
    {
        return new BoolShaderObject(
            $"({a.Expression} == {b.Expression})",
            a.Dependecies.Concat(b.Dependecies)
        );
    }
    
    public static BoolShaderObject operator !=(Vec4ShaderObject a, Vec4ShaderObject b)
    {
        return new BoolShaderObject(
            $"({a.Expression} != {b.Expression})",
            a.Dependecies.Concat(b.Dependecies)
        );
    }

    public static Vec4ShaderObject operator +(Vec4ShaderObject v, Vec4ShaderObject u)
        => new Vec4ShaderObject(
            $"({v} + {u})",
            v.Dependecies.Concat(u.Dependecies)
        );
    
    public static Vec4ShaderObject operator -(Vec4ShaderObject v, Vec4ShaderObject u)
        => new Vec4ShaderObject(
            $"({v} - {u})",
            v.Dependecies.Concat(u.Dependecies)
        );
    
    public static FloatShaderObject operator *(Vec4ShaderObject v, Vec4ShaderObject u)
        => new FloatShaderObject(
            $"({v} * {u})",
            v.Dependecies.Concat(u.Dependecies)
        );

    public static Vec4ShaderObject operator *(Vec4ShaderObject v, FloatShaderObject a)
    {
        var dependecies = v.Dependecies.Concat(a.Dependecies);
        return new ($"({a} * {v})", dependecies);
    }

    public static Vec4ShaderObject operator *(FloatShaderObject a, Vec4ShaderObject v)
    {
        var dependecies = v.Dependecies.Concat(a.Dependecies);
        return new ($"({a} * {v})", dependecies);
    }

    public static Vec4ShaderObject operator /(Vec4ShaderObject v, FloatShaderObject a)
    {
        var dependecies = v.Dependecies.Concat(a.Dependecies);
        return new ($"({v} / {a})", dependecies);
    }
    
    public static Vec4ShaderObject operator +(Vec4ShaderObject v, 
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z, FloatShaderObject w) tuple)
    {
        Vec4ShaderObject u = tuple;
        return v + u;
    }

    public static Vec4ShaderObject operator +(
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z, FloatShaderObject w) tuple, 
        Vec4ShaderObject v)
    {
        Vec4ShaderObject u = tuple;
        return v + u;
    }
    
    public static Vec4ShaderObject operator -(Vec4ShaderObject v,
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z, FloatShaderObject w) tuple)
    {
        Vec4ShaderObject u = tuple;
        return v - u;
    }

    public static Vec4ShaderObject operator -(
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z, FloatShaderObject w) tuple, 
        Vec4ShaderObject v)
    {
        Vec4ShaderObject u = tuple;
        return v - u;
    }
    
    public static FloatShaderObject operator *(Vec4ShaderObject v, 
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z, FloatShaderObject w) tuple)
    {
        Vec4ShaderObject u = tuple;
        return v * u;
    }

    public static FloatShaderObject operator *(
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z, FloatShaderObject w) tuple,
        Vec4ShaderObject v)
    {
        Vec4ShaderObject u = tuple;
        return v * u;
    }

    public static implicit operator Vec4ShaderObject((float x, float y, float z, float w) tuple)
        => new Vec4ShaderObject($"vec4({tuple.x.Format()}, {tuple.y.Format()}, {tuple.z.Format()}, {tuple.w.Format()})");

    public static implicit operator Vec4ShaderObject((FloatShaderObject x, FloatShaderObject y, FloatShaderObject z, FloatShaderObject w) tuple)
        => new Vec4ShaderObject(
            $"vec4({tuple.x.Expression}, {tuple.y.Expression}, {tuple.z.Expression}, {tuple.w.Expression})",
            tuple.x.Dependecies.Concat(tuple.y.Dependecies).Concat(tuple.z.Dependecies).Concat(tuple.w.Dependecies)
        );
    
    public static implicit operator Vec4ShaderObject((Vec2ShaderObject xy, FloatShaderObject z, FloatShaderObject w) tuple)
        => new Vec4ShaderObject(
            $"vec4({tuple.xy.Expression}, {tuple.z.Expression}, {tuple.w.Expression})",
            tuple.xy.Dependecies.Concat(tuple.z.Dependecies).Concat(tuple.w.Dependecies)
        );
    
    public static implicit operator Vec4ShaderObject((FloatShaderObject x, Vec2ShaderObject yz, FloatShaderObject w) tuple)
        => new Vec4ShaderObject(
            $"vec4({tuple.x.Expression}, {tuple.yz.Expression}, {tuple.w.Expression})",
            tuple.x.Dependecies.Concat(tuple.yz.Dependecies).Concat(tuple.w.Dependecies)
        );
    
    public static implicit operator Vec4ShaderObject((FloatShaderObject x, FloatShaderObject y, Vec2ShaderObject zw) tuple)
        => new Vec4ShaderObject(
            $"vec4({tuple.x.Expression}, {tuple.y.Expression}, {tuple.zw.Expression})",
            tuple.x.Dependecies.Concat(tuple.y.Dependecies).Concat(tuple.zw.Dependecies)
        );
    
    public static implicit operator Vec4ShaderObject((Vec2ShaderObject xy, Vec2ShaderObject zw) tuple)
        => new Vec4ShaderObject(
            $"vec4({tuple.xy.Expression}, {tuple.zw.Expression})",
            tuple.xy.Dependecies.Concat(tuple.zw.Dependecies)
        );

    public static implicit operator Vec4ShaderObject((Vec3ShaderObject xyz, FloatShaderObject w) tuple)
        => new Vec4ShaderObject(
            $"vec4({tuple.xyz.Expression}, {tuple.w.Expression})",
            tuple.xyz.Dependecies.Concat(tuple.w.Dependecies)
        );
    
    public static implicit operator Vec4ShaderObject((FloatShaderObject x, Vec3ShaderObject yzw) tuple)
        => new Vec4ShaderObject(
            $"vec4({tuple.x.Expression}, {tuple.yzw.Expression})",
            tuple.x.Dependecies.Concat(tuple.yzw.Dependecies)
        );
    
    public static Vec4ShaderObject operator +(Vec4ShaderObject x)
    {
        var dependecies = x.Dependecies;
        return new ($"(+{x})", dependecies);
    }
    
    public static Vec4ShaderObject operator -(Vec4ShaderObject x)
    {
        var dependecies = x.Dependecies;
        return new ($"(-{x})", dependecies);
    }
}