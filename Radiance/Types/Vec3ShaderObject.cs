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
/// Represent a Vec3 data in shader implementation.
/// </summary>
public class vec3(
    string value,
    ShaderOrigin origin,
    IEnumerable<ShaderDependence> deps)
    : ShaderObject(ShaderType.Vec3, value, origin, deps)
{
    public val this[int index]
        => Transform<vec3, val>(
            $"({this}[{index}])", this
        );

    public val x
        => Transform<vec3, val>(
            $"({this}.x)", this
        );
    
    public val y
        => Transform<vec3, val>(
            $"({this}.y)", this
        );
    
    public val z
        => Transform<vec3, val>(
            $"({this}.z)", this
        );
    
    public static boolean operator ==(vec3 a, vec3 b)
        => Union<boolean>($"({a} == {b})", a, b);
    
    public static boolean operator !=(vec3 a, vec3 b)
        => Union<boolean>($"({a} != {b})", a, b);

    public static vec3 operator +(vec3 v, vec3 u)
        => Union<vec3>($"({v} + {u})", v, u);
    
    public static vec3 operator -(vec3 v, vec3 u)
        => Union<vec3>($"({v} - {u})", v, u);
    
    public static vec3 operator *(vec3 v, vec3 u)
        => Union<vec3>($"({v} * {u})", v, u);
    
    public static vec3 operator +(vec3 v, 
        (val x, val y, val z) tuple)
    {
        vec3 u = tuple;
        return v + u;
    }

    public static vec3 operator +(
        (val x, val y, val z) tuple, vec3 v)
    {
        vec3 u = tuple;
        return v + u;
    }
    
    public static vec3 operator -(vec3 v, 
        (val x, val y, val z) tuple)
    {
        vec3 u = tuple;
        return v - u;
    }

    public static vec3 operator -(
        (val x, val y, val z) tuple, vec3 v)
    {
        vec3 u = tuple;
        return v - u;
    }
    
    public static vec3 operator *(vec3 v, 
        (val x, val y, val z) tuple)
    {
        vec3 u = tuple;
        return v * u;
    }

    public static vec3 operator *(
        (val x, val y, val z) tuple, vec3 v)
    {
        vec3 u = tuple;
        return v * u;
    }

    public static vec3 operator *(vec3 v, val a)
        => Union<vec3>($"({a} * {v})", v, a);

    public static vec3 operator *(val a, vec3 v)
        => Union<vec3>($"({a} * {v})", v, a);

    public static vec3 operator /(vec3 v, val a)
        => Union<vec3>($"({v} / {a})", v, a);
    
    public static implicit operator vec3((float x, float y, float z) tuple)
        => new ($"vec3({tuple.x.ToString(CultureInfo.InvariantCulture)}, {tuple.y.ToString(CultureInfo.InvariantCulture)}, " + 
            $"{tuple.z.ToString(CultureInfo.InvariantCulture)})", ShaderOrigin.Global, []);

    public static implicit operator vec3(
        (val x, val y, val z) tuple)
        => Union<vec3>($"vec3({tuple.x}, {tuple.y}, {tuple.z})", tuple.x, tuple.y, tuple.z);
    
    public static vec3 operator +(vec3 x)
        => Transform<vec3, vec3>($"(+{x})", x);
    
    public static vec3 operator -(vec3 x)
        => Transform<vec3, vec3>($"(-{x})", x);
}