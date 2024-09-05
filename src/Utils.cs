/* Author:  Leonardo Trevisan Silio
 * Date:    28/08/2024
 */
#pragma warning disable IDE1006

using System;
using System.Text;

namespace Radiance;

using Renders;
using Primitives;
using Exceptions;
using Shaders;
using Shaders.Objects;
using Shaders.Dependencies;

/// <summary>
/// A facade with all utils to use Radiance features.
/// </summary>
public static class Utils
{
    internal readonly static PixelDependence pixelDep = new();
    internal readonly static BufferDependence bufferDep = new();
    internal readonly static WidthWindowDependence widthDep = new();
    internal readonly static HeightWindowDependence heightDep = new();
    
    /// <summary>
    /// Get (1, 0, 0) vector.
    /// </summary>
    public static readonly Vec3 i = new(1, 0, 0); 
    
    /// <summary>
    /// Get (0, 1, 0) vector.
    /// </summary>
    public static readonly Vec3 j = new(0, 1, 0);

    /// <summary>
    /// Get (0, 0, 1) vector.
    /// </summary>
    public static readonly Vec3 k = new(0, 0, 1);

    /// <summary>
    /// Get (0, 0, 0) origin vector.
    /// </summary>
    public static readonly Vec3 origin = new(0, 0, 0);

    public static readonly Vec4 red = new(1, 0, 0, 1);
    public static readonly Vec4 green = new(0, 1, 0, 1);
    public static readonly Vec4 blue = new(0, 0, 1, 1);
    public static readonly Vec4 yellow = new(1, 1, 0, 1);
    public static readonly Vec4 black = new(0, 0, 0, 1);
    public static readonly Vec4 white = new(1, 1, 1, 1);
    public static readonly Vec4 cyan = new(0, 1, 1, 1);
    public static readonly Vec4 magenta = new(1, 0, 1, 1);

    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static dynamic render(Action function)
    {
        ArgumentNullException.ThrowIfNull(function, nameof(function));
        
        var render = new Render(function);
        render.Load();

        return render;
    }

    /// <summary>
    /// Get ou update the actual position of a generic point of the drawed polygon.
    /// Shader Only.
    /// </summary>
    public static Vec3ShaderObject pos
    {
        get
        {
            var ctx = RenderContext.GetContext()
                ?? throw new ShaderOnlyResourceException();
            return ctx.Position;
        }
        set
        {
            var ctx = RenderContext.GetContext()
                ?? throw new ShaderOnlyResourceException();
            ctx.Position = value;
        }
    }

    /// <summary>
    /// Get the x position of pixel.
    /// </summary>
    public static readonly FloatShaderObject x = new(
        "pixelPos.x", ShaderOrigin.FragmentShader, [pixelDep, bufferDep]
    );

    /// <summary>
    /// Get the y position of pixel.
    /// </summary>
    public static readonly FloatShaderObject y = new(
        "pixelPos.y", ShaderOrigin.FragmentShader, [pixelDep, bufferDep]
    );

    /// <summary>
    /// Get the z position of pixel.
    /// </summary>
    public static readonly FloatShaderObject z = new(
        "pixelPos.z", ShaderOrigin.FragmentShader, [pixelDep, bufferDep]
    );

    /// <summary>
    /// Get ou update the actual color of a generic point inside drawed area.
    /// Shader Only.
    /// </summary>
    public static Vec4ShaderObject color
    {
        get
        {
            var ctx = RenderContext.GetContext()
                ?? throw new ShaderOnlyResourceException();
            return ctx.Color;
        }
        set
        {
            var ctx = RenderContext.GetContext()
                ?? throw new ShaderOnlyResourceException();
            var variable = new VariableDependence(value);
            ctx.Color = new Vec4ShaderObject(
                variable.Name, ShaderOrigin.FragmentShader,
                [..value.Dependencies, variable]
            );
        }
    }
    
    /// <summary>
    /// Returns the cosine of the specified angle.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject cos(FloatShaderObject angle)
        => func<FloatShaderObject>("cos", angle);

    /// <summary>
    /// Returns the sine of the specified angle.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject sin(FloatShaderObject angle)
        => func<FloatShaderObject>("sin", angle);

    /// <summary>
    /// Returns the tangent of the specified angle.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject tan(FloatShaderObject angle)
        => func<FloatShaderObject>("tan", angle);

    /// <summary>
    /// Returns the exponential (e^x) of the specified value.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject exp(FloatShaderObject value)
        => func<FloatShaderObject>("exp", value);

    /// <summary>
    /// Returns the exponential (2^x) of the specified value.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject exp2(FloatShaderObject value)
        => func<FloatShaderObject>("exp2", value);

    /// <summary>
    /// Returns the logarithm (base e) of the specified value.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject log(FloatShaderObject value)
        => func<FloatShaderObject>("log", value);

    /// <summary>
    /// Returns the logarithm (base 2) of the specified value.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject log2(FloatShaderObject value)
        => func<FloatShaderObject>("log2", value);

    /// <summary>
    /// Perform Hermite interpolation between two values.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject smoothstep(
        FloatShaderObject edge0,
        FloatShaderObject edge1,
        FloatShaderObject x
    )  => func<FloatShaderObject>("smoothstep", edge0, edge1, x);

    /// <summary>
    /// Generate a step function by comparing two values.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject step(
        FloatShaderObject edge0,
        FloatShaderObject x
    )  => func<FloatShaderObject>("step", edge0, x);
    
    /// <summary>
    /// Generate a step function by comparing two values.
    /// Shader Only.
    /// </summary>
    public static Vec2ShaderObject step(
        Vec2ShaderObject edge0,
        Vec2ShaderObject x
    )  => func<Vec2ShaderObject>("step", edge0, x);
    
    /// <summary>
    /// Generate a step function by comparing two values.
    /// Shader Only.
    /// </summary>
    public static Vec3ShaderObject step(
        Vec3ShaderObject edge0,
        Vec3ShaderObject x
    )  => func<Vec3ShaderObject>("step", edge0, x);
    
    /// <summary>
    /// Calculate the length of a vector.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject length(Vec2ShaderObject vec) 
        => func<FloatShaderObject>("length", vec);

    /// <summary>
    /// Calculate the length of a vector.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject length(Vec3ShaderObject vec) 
        => func<FloatShaderObject>("length", vec);

    /// <summary>
    /// Calculate the distance between two points.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject distance(Vec2ShaderObject p0, Vec2ShaderObject p1)
        => func<FloatShaderObject>("distance", p0, p1);

    /// <summary>
    /// Calculate the distance between two points.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject distance(Vec3ShaderObject p0, Vec3ShaderObject p1)
        => func<FloatShaderObject>("distance", p0, p1);

    /// <summary>
    /// Calculate the dot product of two vectors.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject dot(Vec3ShaderObject v0, Vec3ShaderObject v1) 
        => func<FloatShaderObject>("dot", v0, v1);

    /// <summary>
    /// Calculate the dot product of two vectors.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject dot(Vec2ShaderObject v0, Vec2ShaderObject v1)
        => func<FloatShaderObject>("dot", v0, v1);

    /// <summary>
    /// Calculate the cross product of two vectors.
    /// Shader Only.
    /// </summary>
    public static Vec3ShaderObject cross(Vec3ShaderObject v0, Vec3ShaderObject v1) 
        => func<Vec3ShaderObject>("cross", v0, v1);
    
    /// <summary>
    /// Find the nearest integer to the parameter.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject round(FloatShaderObject angle)
        => func<FloatShaderObject>("round", angle);

    /// <summary>
    /// Find the nearest integer less than or equal to the parameter.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject floor(FloatShaderObject angle)
        => func<FloatShaderObject>("floor", angle);

    /// <summary>
    /// Find the nearest integer that is greater than or equal to the parameter.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject ceil(FloatShaderObject angle)
        => func<FloatShaderObject>("ceil", angle);

    /// <summary>
    /// Find the truncated value of the parameter.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject trunc(FloatShaderObject angle)
        => func<FloatShaderObject>("trunc", angle);

    /// <summary>
    /// Return the greater of two values.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject max(FloatShaderObject x, FloatShaderObject y)
        => func<FloatShaderObject>("max", x, y);
    
    /// <summary>
    /// Return the lesser of two values.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject min(FloatShaderObject x, FloatShaderObject y)
        => func<FloatShaderObject>("min", x, y);
    
    /// <summary>
    /// Linearly interpolate between two values.
    /// Shader Only.
    /// </summary>
    public static T mix<T>(T x, T y, FloatShaderObject a) 
        where T : ShaderObject
        => func<T>("mix", x, y, a);

    /// <summary>
    /// Open a image file to use in your shader.
    /// </summary>
    public static Texture open(string imgFile)
    {
        var texture = new Texture(imgFile);
        return texture;
    }

    /// <summary>
    /// Get a pixel color of a img in a specific position of a texture.
    /// </summary>
    public static Vec4ShaderObject texture(Sampler2DShaderObject img, Vec2ShaderObject pos)
        => autoVar(func<Vec4ShaderObject>("texture", img, pos));
    
    static FloatShaderObject var(FloatShaderObject obj, string name)
        => new (name, obj.Origin, [..obj.Dependencies, new VariableDependence(
            obj.Type.TypeName, name, obj.Expression
        )]);

    static Vec2ShaderObject var(Vec2ShaderObject obj, string name)
        => new (name, obj.Origin, [..obj.Dependencies, new VariableDependence(
            obj.Type.TypeName, name, obj.Expression
        )]);

    static Vec3ShaderObject var(Vec3ShaderObject obj, string name)
        => new (name, obj.Origin, [..obj.Dependencies, new VariableDependence(
            obj.Type.TypeName, name, obj.Expression
        )]);

    static Vec4ShaderObject var(Vec4ShaderObject obj, string name)
        => new (name, obj.Origin, [..obj.Dependencies, new VariableDependence(
            obj.Type.TypeName, name, obj.Expression
        )]);
    
    static FloatShaderObject autoVar(FloatShaderObject obj)
    {
        var variable = new VariableDependence(obj);
        return new (variable.Name, obj.Origin, [..obj.Dependencies, variable]);
    }

    static Vec2ShaderObject autoVar(Vec2ShaderObject obj)
    {
        var variable = new VariableDependence(obj);
        return new (variable.Name, obj.Origin, [..obj.Dependencies, variable]);
    }

    static Vec3ShaderObject autoVar(Vec3ShaderObject obj)
    {
        var variable = new VariableDependence(obj);
        return new (variable.Name, obj.Origin, [..obj.Dependencies, variable]);
    }

    static Vec4ShaderObject autoVar(Vec4ShaderObject obj)
    {
        var variable = new VariableDependence(obj);
        return new (variable.Name, obj.Origin, [..obj.Dependencies, variable]);
    }

    static R func<R>(string name, params ShaderObject[] objs)
        where R : ShaderObject => ShaderObject.Union<R>(buildObject(name, objs), objs);

    static string buildObject(
        string funcName,
        params object[] inputs
    )
    {
        var sb = new StringBuilder();
        sb.Append(funcName);
        sb.Append('(');

        for (int i = 0; i < inputs.Length - 1; i++)
        {
            if (inputs[i] is ShaderObject input)
                sb.Append(input.Expression);
            sb.Append(',');
            sb.Append(' ');
        }

        if (inputs.Length > 0)
        {
            if (inputs[^1] is ShaderObject input)
                sb.Append(input.Expression);
            sb.Append(')');
        }
            
        return sb.ToString();
    }
}