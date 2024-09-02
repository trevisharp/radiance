/* Author:  Leonardo Trevisan Silio
 * Date:    25/01/2024
 */
#pragma warning disable CS0660
#pragma warning disable CS0661

using System.Globalization;
using System.Collections.Generic;

namespace Radiance.Shaders.Objects;

/// <summary>
/// Represent a Vec4 data in shader implementation.
/// </summary>
public class Vec4ShaderObject(
    string value,
    ShaderOrigin origin,
    IEnumerable<ShaderDependence> deps)
    : ShaderObject(ShaderType.Vec4, value, origin, deps)
{
    public FloatShaderObject this[int index]
        => Transform<Vec4ShaderObject, FloatShaderObject>(
            $"({this}[{index}])", this
        );

    public FloatShaderObject x
        => Transform<Vec4ShaderObject, FloatShaderObject>(
            $"({this}.x)", this
        );
    
    public FloatShaderObject y
        => Transform<Vec4ShaderObject, FloatShaderObject>(
            $"({this}.y)", this
        );
    
    public FloatShaderObject z
        => Transform<Vec4ShaderObject, FloatShaderObject>(
            $"({this}.z)", this
        );
    
    public FloatShaderObject w
        => Transform<Vec4ShaderObject, FloatShaderObject>(
            $"({this}.w)", this
        );
    
    public static BoolShaderObject operator ==(Vec4ShaderObject a, Vec4ShaderObject b)
        => Union<BoolShaderObject>($"({a} == {b})", a, b);
    
    public static BoolShaderObject operator !=(Vec4ShaderObject a, Vec4ShaderObject b)
        => Union<BoolShaderObject>($"({a} != {b})", a, b);

    public static Vec4ShaderObject operator +(Vec4ShaderObject v, Vec4ShaderObject u)
        => Union<Vec4ShaderObject>($"({v} + {u})", v, u);
    
    public static Vec4ShaderObject operator -(Vec4ShaderObject v, Vec4ShaderObject u)
        => Union<Vec4ShaderObject>($"({v} - {u})", v, u);
    
    public static Vec4ShaderObject operator *(Vec4ShaderObject v, Vec4ShaderObject u)
        => Union<Vec4ShaderObject>($"({v} * {u})", v, u);
    
    public static Vec4ShaderObject operator +(Vec4ShaderObject v, 
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z, FloatShaderObject w) tuple)
    {
        Vec4ShaderObject u = tuple;
        return v + u;
    }

    public static Vec4ShaderObject operator +(
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z, FloatShaderObject w) tuple, Vec4ShaderObject v)
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
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z, FloatShaderObject w) tuple, Vec4ShaderObject v)
    {
        Vec4ShaderObject u = tuple;
        return v - u;
    }
    
    public static Vec4ShaderObject operator *(Vec4ShaderObject v, 
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z, FloatShaderObject w) tuple)
    {
        Vec4ShaderObject u = tuple;
        return v * u;
    }

    public static Vec4ShaderObject operator *(
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z, FloatShaderObject w) tuple, Vec4ShaderObject v)
    {
        Vec4ShaderObject u = tuple;
        return v * u;
    }

    public static Vec4ShaderObject operator *(Vec4ShaderObject v, FloatShaderObject a)
        => Union<Vec4ShaderObject>($"({a} * {v})", v, a);

    public static Vec4ShaderObject operator *(FloatShaderObject a, Vec4ShaderObject v)
        => Union<Vec4ShaderObject>($"({a} * {v})", v, a);

    public static Vec4ShaderObject operator /(Vec4ShaderObject v, FloatShaderObject a)
        => Union<Vec4ShaderObject>($"({v} / {a})", v, a);
    
    public static implicit operator Vec4ShaderObject((float x, float y, float z, float w) tuple)
        => new ($"vec4({tuple.x.ToString(CultureInfo.InvariantCulture)}, {tuple.y.ToString(CultureInfo.InvariantCulture)}, " 
            +$"{tuple.z.ToString(CultureInfo.InvariantCulture)}, {tuple.w.ToString(CultureInfo.InvariantCulture)})", ShaderOrigin.Global, []);

    public static implicit operator Vec4ShaderObject(
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z, FloatShaderObject w) tuple)
        => Union<Vec4ShaderObject>($"vec4({tuple.x}, {tuple.y}, {tuple.z}, {tuple.w})", tuple.x, tuple.y, tuple.z, tuple.w);
    
    public static Vec4ShaderObject operator +(Vec4ShaderObject x)
        => Transform<Vec4ShaderObject, Vec4ShaderObject>($"(+{x})", x);
    
    public static Vec4ShaderObject operator -(Vec4ShaderObject x)
        => Transform<Vec4ShaderObject, Vec4ShaderObject>($"(-{x})", x);
}