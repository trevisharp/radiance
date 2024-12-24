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
/// Represent a Vec4 data in shader implementation.
/// </summary>
public class vec4(
    string value,
    ShaderOrigin origin,
    IEnumerable<ShaderDependence> deps)
    : ShaderObject(ShaderType.Vec4, value, origin, deps)
{
    public val this[int index]
        => Transform<vec4, val>(
            $"({this}[{index}])", this
        );

    public val x
        => Transform<vec4, val>(
            $"({this}.x)", this
        );
    
    public val y
        => Transform<vec4, val>(
            $"({this}.y)", this
        );
    
    public val z
        => Transform<vec4, val>(
            $"({this}.z)", this
        );
    
    public val w
        => Transform<vec4, val>(
            $"({this}.w)", this
        );
    
    public static boolean operator ==(vec4 a, vec4 b)
        => Union<boolean>($"({a} == {b})", a, b);
    
    public static boolean operator !=(vec4 a, vec4 b)
        => Union<boolean>($"({a} != {b})", a, b);

    public static vec4 operator +(vec4 v, vec4 u)
        => Union<vec4>($"({v} + {u})", v, u);
    
    public static vec4 operator -(vec4 v, vec4 u)
        => Union<vec4>($"({v} - {u})", v, u);
    
    public static vec4 operator *(vec4 v, vec4 u)
        => Union<vec4>($"({v} * {u})", v, u);
    
    public static vec4 operator +(vec4 v, 
        (val x, val y, val z, val w) tuple)
    {
        vec4 u = tuple;
        return v + u;
    }

    public static vec4 operator +(
        (val x, val y, val z, val w) tuple, vec4 v)
    {
        vec4 u = tuple;
        return v + u;
    }
    
    public static vec4 operator -(vec4 v, 
        (val x, val y, val z, val w) tuple)
    {
        vec4 u = tuple;
        return v - u;
    }

    public static vec4 operator -(
        (val x, val y, val z, val w) tuple, vec4 v)
    {
        vec4 u = tuple;
        return v - u;
    }
    
    public static vec4 operator *(vec4 v, 
        (val x, val y, val z, val w) tuple)
    {
        vec4 u = tuple;
        return v * u;
    }

    public static vec4 operator *(
        (val x, val y, val z, val w) tuple, vec4 v)
    {
        vec4 u = tuple;
        return v * u;
    }

    public static vec4 operator *(vec4 v, val a)
        => Union<vec4>($"({a} * {v})", v, a);

    public static vec4 operator *(val a, vec4 v)
        => Union<vec4>($"({a} * {v})", v, a);

    public static vec4 operator /(vec4 v, val a)
        => Union<vec4>($"({v} / {a})", v, a);
    
    public static implicit operator vec4((float x, float y, float z, float w) tuple)
        => new ($"vec4({tuple.x.ToString(CultureInfo.InvariantCulture)}, {tuple.y.ToString(CultureInfo.InvariantCulture)}, " 
            +$"{tuple.z.ToString(CultureInfo.InvariantCulture)}, {tuple.w.ToString(CultureInfo.InvariantCulture)})", ShaderOrigin.Global, []);

    public static implicit operator vec4(
        (val x, val y, val z, val w) tuple)
        => Union<vec4>($"vec4({tuple.x}, {tuple.y}, {tuple.z}, {tuple.w})", tuple.x, tuple.y, tuple.z, tuple.w);
    
    public static vec4 operator +(vec4 x)
        => Transform<vec4, vec4>($"(+{x})", x);
    
    public static vec4 operator -(vec4 x)
        => Transform<vec4, vec4>($"(-{x})", x);
}