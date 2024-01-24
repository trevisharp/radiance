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
/// Represent a Vec2 data in shader implementation.
/// </summary>
public class Vec2ShaderObject : ShaderObject
{
    public Vec2ShaderObject()
    {
        this.Expression = "(0.0, 0.0)";
        this.Dependecies = new OldShaderDependence[0];
        this.Type = ShaderType.Vec2;
    }

    public Vec2ShaderObject(string value, params OldShaderDependence[] dependecies)
    {
        this.Expression = value;
        this.Dependecies = dependecies;
        this.Type = ShaderType.Vec2;
    }

    public Vec2ShaderObject(string value, IEnumerable<OldShaderDependence> dependecies)
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
    
    public static BoolShaderObject operator ==(Vec2ShaderObject a, Vec2ShaderObject b)
    {
        return new BoolShaderObject(
            $"({a.Expression} == {b.Expression})",
            a.Dependecies.Concat(b.Dependecies)
        );
    }
    
    public static BoolShaderObject operator !=(Vec2ShaderObject a, Vec2ShaderObject b)
    {
        return new BoolShaderObject(
            $"({a.Expression} != {b.Expression})",
            a.Dependecies.Concat(b.Dependecies)
        );
    }

    public static Vec2ShaderObject operator +(Vec2ShaderObject v, Vec2ShaderObject u)
        => new Vec2ShaderObject(
            $"({v} + {u})",
            v.Dependecies.Concat(u.Dependecies)
        );
    
    public static Vec2ShaderObject operator -(Vec2ShaderObject v, Vec2ShaderObject u)
        => new Vec2ShaderObject(
            $"({v} - {u})",
            v.Dependecies.Concat(u.Dependecies)
        );
    
    public static FloatShaderObject operator *(Vec2ShaderObject v, Vec2ShaderObject u)
        => new FloatShaderObject(
            $"({v} * {u})",
            v.Dependecies.Concat(u.Dependecies)
        );
    
    public static Vec2ShaderObject operator +(Vec2ShaderObject v, (FloatShaderObject x, FloatShaderObject y) tuple)
    {
        Vec2ShaderObject u = tuple;
        return v + u;
    }

    public static Vec2ShaderObject operator +((FloatShaderObject x, FloatShaderObject y) tuple, Vec2ShaderObject v)
    {
        Vec2ShaderObject u = tuple;
        return v + u;
    }
    
    public static Vec2ShaderObject operator -(Vec2ShaderObject v, (FloatShaderObject x, FloatShaderObject y) tuple)
    {
        Vec2ShaderObject u = tuple;
        return v - u;
    }

    public static Vec2ShaderObject operator -((FloatShaderObject x, FloatShaderObject y) tuple, Vec2ShaderObject v)
    {
        Vec2ShaderObject u = tuple;
        return v - u;
    }
    
    public static FloatShaderObject operator *(Vec2ShaderObject v, (FloatShaderObject x, FloatShaderObject y) tuple)
    {
        Vec2ShaderObject u = tuple;
        return v * u;
    }

    public static FloatShaderObject operator *((FloatShaderObject x, FloatShaderObject y) tuple, Vec2ShaderObject v)
    {
        Vec2ShaderObject u = tuple;
        return v * u;
    }

    public static Vec2ShaderObject operator *(Vec2ShaderObject v, FloatShaderObject a)
    {
        var dependecies = v.Dependecies.Concat(a.Dependecies);
        return new ($"({a} * {v})", dependecies);
    }

    public static Vec2ShaderObject operator *(FloatShaderObject a, Vec2ShaderObject v)
    {
        var dependecies = v.Dependecies.Concat(a.Dependecies);
        return new ($"({a} * {v})", dependecies);
    }

    public static Vec2ShaderObject operator /(Vec2ShaderObject v, FloatShaderObject a)
    {
        var dependecies = v.Dependecies.Concat(a.Dependecies);
        return new ($"({v} / {a})", dependecies);
    }
    
    public static implicit operator Vec2ShaderObject((float x, float y) tuple)
        => new Vec2ShaderObject($"vec2({tuple.x.Format()}, {tuple.y.Format()})");

    public static implicit operator Vec2ShaderObject((FloatShaderObject x, FloatShaderObject y) tuple)
        => new Vec2ShaderObject(
            $"vec2({tuple.x.Expression}, {tuple.y.Expression})",
            tuple.x.Dependecies.Concat(tuple.y.Dependecies)
        );
    
    public static Vec2ShaderObject operator +(Vec2ShaderObject x)
    {
        var dependecies = x.Dependecies;
        return new ($"(+{x})", dependecies);
    }
    
    public static Vec2ShaderObject operator -(Vec2ShaderObject x)
    {
        var dependecies = x.Dependecies;
        return new ($"(-{x})", dependecies);
    }
}