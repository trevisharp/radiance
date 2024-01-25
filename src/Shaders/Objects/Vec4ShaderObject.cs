/* Author:  Leonardo Trevisan Silio
 * Date:    25/01/2024
 */
#pragma warning disable CS0660
#pragma warning disable CS0661
using System.Collections.Generic;

namespace Radiance.Shaders.Objects;

using Internal;

/// <summary>
/// Represent a Vec4 data in shader implementation.
/// </summary>
public class Vec4ShaderObject : ShaderObject
{
    public Vec4ShaderObject(
        string value, ShaderOrigin origin,
        IEnumerable<ShaderDependence> deps
        ) : base(ShaderType.Vec3, value, origin, deps) { }

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
        => Union<Vec4ShaderObject, Vec4ShaderObject, BoolShaderObject>(
            $"({a} == {b})", a, b);
    
    public static BoolShaderObject operator !=(Vec4ShaderObject a, Vec4ShaderObject b)
        => Union<Vec4ShaderObject, Vec4ShaderObject, BoolShaderObject>(
            $"({a} != {b})", a, b);

    public static Vec4ShaderObject operator +(Vec4ShaderObject v, Vec4ShaderObject u)
        => Union($"({v} + {u})", v, u);
    
    public static Vec4ShaderObject operator -(Vec4ShaderObject v, Vec4ShaderObject u)
        => Union($"({v} - {u})", v, u);
    
    public static FloatShaderObject operator *(Vec4ShaderObject v, Vec4ShaderObject u)
        => Union<Vec4ShaderObject, Vec4ShaderObject, FloatShaderObject>(
            $"({v} == {u})", v, u);
    
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
    
    public static FloatShaderObject operator *(Vec4ShaderObject v, 
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z, FloatShaderObject w) tuple)
    {
        Vec4ShaderObject u = tuple;
        return v * u;
    }

    public static FloatShaderObject operator *(
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z, FloatShaderObject w) tuple, Vec4ShaderObject v)
    {
        Vec4ShaderObject u = tuple;
        return v * u;
    }

    public static Vec4ShaderObject operator *(Vec4ShaderObject v, FloatShaderObject a)
        => Union<Vec4ShaderObject, FloatShaderObject, Vec4ShaderObject>(
            $"({a} * {v})", v, a);

    public static Vec4ShaderObject operator *(FloatShaderObject a, Vec4ShaderObject v)
        => Union<Vec4ShaderObject, FloatShaderObject, Vec4ShaderObject>(
            $"({a} * {v})", v, a);

    public static Vec4ShaderObject operator /(Vec4ShaderObject v, FloatShaderObject a)
        => Union<Vec4ShaderObject, FloatShaderObject, Vec4ShaderObject>(
            $"({v} / {a})", v, a);
    
    public static implicit operator Vec4ShaderObject((float x, float y, float z, float w) tuple)
        => new ($"vec2({tuple.x.Format()}, {tuple.y.Format()},  {tuple.z.Format()}, {tuple.w.Format()})", ShaderOrigin.Global, []);

    public static implicit operator Vec4ShaderObject(
        (FloatShaderObject x, FloatShaderObject y, FloatShaderObject z, FloatShaderObject w) tuple)
        => new ($"vec2({tuple.x}, {tuple.y}, {tuple.z}, {tuple.w})", ShaderOrigin.Global, []);
    
    public static Vec4ShaderObject operator +(Vec4ShaderObject x)
        => Transform<Vec4ShaderObject, Vec4ShaderObject>($"(+{x})", x);
    
    public static Vec4ShaderObject operator -(Vec4ShaderObject x)
        => Transform<Vec4ShaderObject, Vec4ShaderObject>($"(-{x})", x);
}