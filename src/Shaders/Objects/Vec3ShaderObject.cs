/* Author:  Leonardo Trevisan Silio
 * Date:    25/01/2024
 */
#pragma warning disable CS0660
#pragma warning disable CS0661

using System.Globalization;
using System.Collections.Generic;

namespace Radiance.Shaders.Objects;

/// <summary>
/// Represent a Vec3 data in shader implementation.
/// </summary>
public class Vec3ShaderObject(
    string value,
    ShaderOrigin origin,
    IEnumerable<ShaderDependence> deps)
    : ShaderObject(ShaderType.Vec3, value, origin, deps)
{
    public FloatShaderObject this[int index]
        => Transform<Vec3ShaderObject, FloatShaderObject>(
            $"({this}[{index}])", this
        );

    public FloatShaderObject x
        => Transform<Vec3ShaderObject, FloatShaderObject>(
            $"({this}.x)", this
        );
    
    public FloatShaderObject y
        => Transform<Vec3ShaderObject, FloatShaderObject>(
            $"({this}.y)", this
        );
    
    public FloatShaderObject z
        => Transform<Vec3ShaderObject, FloatShaderObject>(
            $"({this}.z)", this
        );
    
    public static BoolShaderObject operator ==(Vec3ShaderObject a, Vec3ShaderObject b)
        => Union<BoolShaderObject>($"({a} == {b})", a, b);
    
    public static BoolShaderObject operator !=(Vec3ShaderObject a, Vec3ShaderObject b)
        => Union<BoolShaderObject>($"({a} != {b})", a, b);

    public static Vec3ShaderObject operator +(Vec3ShaderObject v, Vec3ShaderObject u)
        => Union<Vec3ShaderObject>($"({v} + {u})", v, u);
    
    public static Vec3ShaderObject operator -(Vec3ShaderObject v, Vec3ShaderObject u)
        => Union<Vec3ShaderObject>($"({v} - {u})", v, u);
    
    public static Vec3ShaderObject operator *(Vec3ShaderObject v, Vec3ShaderObject u)
        => Union<Vec3ShaderObject>($"({v} * {u})", v, u);
    
    public static Vec3ShaderObject operator +(Vec3ShaderObject v, 
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z) tuple)
    {
        Vec3ShaderObject u = tuple;
        return v + u;
    }

    public static Vec3ShaderObject operator +(
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z) tuple, Vec3ShaderObject v)
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
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z) tuple, Vec3ShaderObject v)
    {
        Vec3ShaderObject u = tuple;
        return v - u;
    }
    
    public static Vec3ShaderObject operator *(Vec3ShaderObject v, 
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z) tuple)
    {
        Vec3ShaderObject u = tuple;
        return v * u;
    }

    public static Vec3ShaderObject operator *(
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z) tuple, Vec3ShaderObject v)
    {
        Vec3ShaderObject u = tuple;
        return v * u;
    }

    public static Vec3ShaderObject operator *(Vec3ShaderObject v, FloatShaderObject a)
        => Union<Vec3ShaderObject>($"({a} * {v})", v, a);

    public static Vec3ShaderObject operator *(FloatShaderObject a, Vec3ShaderObject v)
        => Union<Vec3ShaderObject>($"({a} * {v})", v, a);

    public static Vec3ShaderObject operator /(Vec3ShaderObject v, FloatShaderObject a)
        => Union<Vec3ShaderObject>($"({v} / {a})", v, a);
    
    public static implicit operator Vec3ShaderObject((float x, float y, float z) tuple)
        => new ($"vec3({tuple.x.ToString(CultureInfo.InvariantCulture)}, {tuple.y.ToString(CultureInfo.InvariantCulture)}, " + 
            $"{tuple.z.ToString(CultureInfo.InvariantCulture)})", ShaderOrigin.Global, []);

    public static implicit operator Vec3ShaderObject(
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z) tuple)
        => Union<Vec3ShaderObject>($"vec3({tuple.x}, {tuple.y}, {tuple.z})", tuple.x, tuple.y, tuple.z);
    
    public static Vec3ShaderObject operator +(Vec3ShaderObject x)
        => Transform<Vec3ShaderObject, Vec3ShaderObject>($"(+{x})", x);
    
    public static Vec3ShaderObject operator -(Vec3ShaderObject x)
        => Transform<Vec3ShaderObject, Vec3ShaderObject>($"(-{x})", x);
}