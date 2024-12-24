/* Author:  Leonardo Trevisan Silio
 * Date:    24/12/2024
 */
#pragma warning disable CS0660
#pragma warning disable CS0661
#pragma warning disable IDE1006
#pragma warning disable IDE0130

using System.Globalization;
using System.Collections.Generic;

namespace Radiance;

using Shaders;

/// <summary>
/// Represent a Vec2 data in shader implementation.
/// </summary>
public class vec2(
    string value,
    ShaderOrigin origin,
    IEnumerable<ShaderDependence> deps) 
    : ShaderObject(ShaderType.Vec2, value, origin, deps)
{
    public val this[int index]
        => Transform<vec2, val>(
            $"({this}[{index}])", this
        );

    public val x
        => Transform<vec2, val>(
            $"({this}.x)", this
        );
    
    public val y
        => Transform<vec2, val>(
            $"({this}.y)", this
        );
    
    public static boolean operator ==(vec2 a, vec2 b)
        => Union<boolean>($"({a} == {b})", a, b);
    
    public static boolean operator !=(vec2 a, vec2 b)
        => Union<boolean>($"({a} != {b})", a, b);

    public static vec2 operator +(vec2 v, vec2 u)
        => Union<vec2>($"({v} + {u})", v, u);
    
    public static vec2 operator -(vec2 v, vec2 u)
        => Union<vec2>($"({v} - {u})", v, u);
    
    public static vec2 operator *(vec2 v, vec2 u)
        => Union<vec2>($"({v} * {u})", v, u);
    
    public static vec2 operator +(vec2 v, (val x, val y) tuple)
    {
        vec2 u = tuple;
        return v + u;
    }

    public static vec2 operator +((val x, val y) tuple, vec2 v)
    {
        vec2 u = tuple;
        return v + u;
    }
    
    public static vec2 operator -(vec2 v, (val x, val y) tuple)
    {
        vec2 u = tuple;
        return v - u;
    }

    public static vec2 operator -((val x, val y) tuple, vec2 v)
    {
        vec2 u = tuple;
        return v - u;
    }
    
    public static vec2 operator *(vec2 v, (val x, val y) tuple)
    {
        vec2 u = tuple;
        return v * u;
    }

    public static vec2 operator *((val x, val y) tuple, vec2 v)
    {
        vec2 u = tuple;
        return v * u;
    }

    public static vec2 operator *(vec2 v, val a)
        => Union<vec2>($"({a} * {v})", v, a);

    public static vec2 operator *(val a, vec2 v)
        => Union<vec2>($"({a} * {v})", v, a);

    public static vec2 operator /(vec2 v, val a)
        => Union<vec2>($"({v} / {a})", v, a);
    
    public static implicit operator vec2((float x, float y) tuple)
        => new ($"vec2({tuple.x.ToString(CultureInfo.InvariantCulture)}, {tuple.y.ToString(CultureInfo.InvariantCulture)})", ShaderOrigin.Global, []);

    public static implicit operator vec2((val x, val y) tuple)
        => Union<vec2>($"vec2({tuple.x}, {tuple.y})", tuple.x, tuple.y);
    
    public static vec2 operator +(vec2 x)
        => Transform<vec2, vec2>($"(+{x})", x);
    
    public static vec2 operator -(vec2 x)
        => Transform<vec2, vec2>($"(-{x})", x);
}