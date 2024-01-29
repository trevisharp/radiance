/* Author:  Leonardo Trevisan Silio
 * Date:    24/01/2024
 */
#pragma warning disable CS0660
#pragma warning disable CS0661
using System.Collections.Generic;

namespace Radiance.Shaders.Objects;

using Internal;

/// <summary>
/// Represent a Vec2 data in shader implementation.
/// </summary>
public class Vec2ShaderObject : ShaderObject
{
    public Vec2ShaderObject(
        string value, ShaderOrigin origin,
        IEnumerable<ShaderDependence> deps
        ) : base(ShaderType.Vec2, value, origin, deps) { }

    public FloatShaderObject this[int index]
        => Transform<Vec2ShaderObject, FloatShaderObject>(
            $"({this}[{index}])", this
        );

    public FloatShaderObject x
        => Transform<Vec2ShaderObject, FloatShaderObject>(
            $"({this}.x)", this
        );
    
    public FloatShaderObject y
        => Transform<Vec2ShaderObject, FloatShaderObject>(
            $"({this}.y)", this
        );
    
    public static BoolShaderObject operator ==(Vec2ShaderObject a, Vec2ShaderObject b)
        => Union<BoolShaderObject>($"({a} == {b})", a, b);
    
    public static BoolShaderObject operator !=(Vec2ShaderObject a, Vec2ShaderObject b)
        => Union<BoolShaderObject>($"({a} != {b})", a, b);

    public static Vec2ShaderObject operator +(Vec2ShaderObject v, Vec2ShaderObject u)
        => Union<Vec2ShaderObject>($"({v} + {u})", v, u);
    
    public static Vec2ShaderObject operator -(Vec2ShaderObject v, Vec2ShaderObject u)
        => Union<Vec2ShaderObject>($"({v} - {u})", v, u);
    
    public static Vec2ShaderObject operator *(Vec2ShaderObject v, Vec2ShaderObject u)
        => Union<Vec2ShaderObject>($"({v} * {u})", v, u);
    
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
    
    public static Vec2ShaderObject operator *(Vec2ShaderObject v, (FloatShaderObject x, FloatShaderObject y) tuple)
    {
        Vec2ShaderObject u = tuple;
        return v * u;
    }

    public static Vec2ShaderObject operator *((FloatShaderObject x, FloatShaderObject y) tuple, Vec2ShaderObject v)
    {
        Vec2ShaderObject u = tuple;
        return v * u;
    }

    public static Vec2ShaderObject operator *(Vec2ShaderObject v, FloatShaderObject a)
        => Union<Vec2ShaderObject>($"({a} * {v})", v, a);

    public static Vec2ShaderObject operator *(FloatShaderObject a, Vec2ShaderObject v)
        => Union<Vec2ShaderObject>($"({a} * {v})", v, a);

    public static Vec2ShaderObject operator /(Vec2ShaderObject v, FloatShaderObject a)
        => Union<Vec2ShaderObject>($"({v} / {a})", v, a);
    
    public static implicit operator Vec2ShaderObject((float x, float y) tuple)
        => new ($"vec2({tuple.x.Format()}, {tuple.y.Format()})", ShaderOrigin.Global, []);

    public static implicit operator Vec2ShaderObject((FloatShaderObject x, FloatShaderObject y) tuple)
        => Union<Vec2ShaderObject>($"vec2({tuple.x}, {tuple.y})", tuple.x, tuple.y);
    
    public static Vec2ShaderObject operator +(Vec2ShaderObject x)
        => Transform<Vec2ShaderObject, Vec2ShaderObject>($"(+{x})", x);
    
    public static Vec2ShaderObject operator -(Vec2ShaderObject x)
        => Transform<Vec2ShaderObject, Vec2ShaderObject>($"(-{x})", x);
}