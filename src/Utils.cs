/* Author:  Leonardo Trevisan Silio
 * Date:    06/09/2024
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

using Float = Shaders.Objects.FloatShaderObject;
using Sampler = Shaders.Objects.Sampler2DShaderObject;

/// <summary>
/// A facade with all utils to use Radiance features.
/// </summary>
public static class Utils
{
    #region  DEPENDENCE UTILS
    internal readonly static TimeDependence timeDep = new();
    internal readonly static PixelDependence pixelDep = new();
    internal readonly static BufferDependence bufferDep = new();
    internal readonly static WidthWindowDependence widthDep = new();
    internal readonly static HeightWindowDependence heightDep = new();

    #endregion

    #region WINDOWS UTILS

    /// <summary>
    /// Get the time between two frames.
    /// </summary>
    public static float dt =>
        Window.IsOpen ? Window.DeltaTime :
        throw new WindowClosedException();

    /// <summary>
    /// A number relatives to 100% to width of viewport.
    /// </summary>
    public static float vw =>
        Window.IsOpen ? Window.Width :
        throw new WindowClosedException();

    /// <summary>
    /// A number relatives to 100% to height of viewport.
    /// </summary>
    public static float vh =>
        Window.IsOpen ? Window.Height :
        throw new WindowClosedException();
    
    #endregion
    
    #region  PRIMITIVE UTILS
    
    /// <summary>
    /// Get a rectangle with size of opened screen centralizated in center of screen.
    /// </summary>
    public static Polygon Screen => 
        Window.IsOpen ?
        Rect(0, 0, 0, Window.Width, Window.Height).ToImmutable() :
        throw new WindowClosedException();

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
    /// Create and get a new empty polygon.
    /// </summary>
    public static Polygon Empty => new MutablePolygon();

    /// <summary>
    /// Get a circle with radius 1 centralizated in (0, 0, 0)
    /// with 128 sides.
    /// </summary>
    public static readonly Polygon Circle = Ellipse(1, 1, 128).ToImmutable();

    /// <summary>
    /// Get a square with side 1 centralizated in (0, 0, 0).
    /// </summary>
    public static readonly Polygon Square = Rect(1, 1).ToImmutable();

    /// <summary>
    /// Create a rectangle with specific width and height
    /// centralized on (0, 0, 0) cordinate.
    /// </summary>
    public static Polygon Rect(float width, float height)
    {
        var halfWid = width / 2;
        var halfHei = height / 2;
        return Empty
            .Add(-halfWid, -halfHei, 0f)
            .Add(-halfHei, halfWid, 0f)
            .Add(halfHei, halfWid, 0f)
            .Add(halfHei, -halfWid, 0f);
    }

    /// <summary>
    /// Create a rectangle with specific width and height
    /// centralized on (x, y, z) cordinate.
    /// </summary>
    public static Polygon Rect(
        float x, float y, float z,
        float width, float height)
    {
        var halfWid = width / 2;
        var halfHei = height / 2;
        return Empty
            .Add(x - halfWid, y - halfHei, z)
            .Add(x - halfWid, y + halfHei, z)
            .Add(x + halfWid, y + halfHei, z)
            .Add(x + halfWid, y - halfHei, z);
    }

    /// <summary>
    /// Create a ellipse with specific a and b radius
    /// centralized on (x, y, z) cordinate with a specific
    /// quantity of sides.
    /// </summary>
    public static MutablePolygon Ellipse(
        float x, float y, float z,
        float a, float b = float.NaN,
        int sizes = 63
    )
    {
        var result = new MutablePolygon();

        float phi = MathF.Tau / sizes;
        if (float.IsNaN(b))
            b = a;

        for (int k = 0; k < sizes; k++)
        {
            result.Add(
                a * MathF.Cos(phi * k) + x,
                b * MathF.Sin(-phi * k) + y,
                z
            );
        }

        return result;
    }

    /// <summary>
    /// Create a ellipse with specific a and b radius
    /// centralized on (0, 0, 0) cordinate with a specific
    /// quantity of sides.
    /// </summary>
    public static MutablePolygon Ellipse(
        float a, float b = float.NaN,
        int sizes = 63
    )
    {
        var result = new MutablePolygon();

        float phi = MathF.Tau / sizes;
        if (float.IsNaN(b))
            b = a;

        for (int k = 0; k < sizes; k++)
        {
            result.Add(
                a * MathF.Cos(phi * k),
                b * MathF.Sin(-phi * k),
                0
            );
        }

        return result;
    }

    /// <summary>
    /// Create a polygon based in recived data.
    /// </summary>
    public static MutablePolygon Data(params Vec3[] vectors)
    {
        var result = new MutablePolygon();

        foreach (var v in vectors)
            result.Add(v.X, v.Y, v.Z);
        
        return result;
    }
    
    /// <summary>
    /// Create a polygon based in recived data.
    /// </summary>
    public static MutablePolygon Data(params Vec2[] vectors)
    {
        var result = new MutablePolygon();

        foreach (var v in vectors)
            result.Add(v.X, v.Y, 0);
        
        return result;
    }

    #endregion

    #region RENDER UTILS
    
    /// <summary>
    /// Get a Kit of autoimplemented renders.
    /// </summary>
    public static RenderKit Kit => RenderKit.Shared;    

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
    /// Create render with shaders based on function recived.
    /// </summary>
    public static dynamic render(Action<Float> function)
    {
        ArgumentNullException.ThrowIfNull(function, nameof(function));
        
        var render = new Render(function);
        render.Load();

        return render;
    }

    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static dynamic render(Action<Float, Float> function)
    {
        ArgumentNullException.ThrowIfNull(function, nameof(function));
        
        var render = new Render(function);
        render.Load();

        return render;
    }

    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static dynamic render(Action<Float, Float, Float> function)
    {
        ArgumentNullException.ThrowIfNull(function, nameof(function));
        
        var render = new Render(function);
        render.Load();

        return render;
    }

    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static dynamic render(Action<Float, Float, Float, Float> function)
    {
        ArgumentNullException.ThrowIfNull(function, nameof(function));
        
        var render = new Render(function);
        render.Load();

        return render;
    }

    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static dynamic render(Action<Sampler> function)
    {
        ArgumentNullException.ThrowIfNull(function, nameof(function));
        
        var render = new Render(function);
        render.Load();

        return render;
    }

    #endregion

    #region SHADER ONLY UTILS

    /// <summary>
    /// Return the current width of screen.
    /// Shader Only.
    /// </summary>
    public static readonly Float width =
        new("width", ShaderOrigin.Global, [widthDep]);

    /// <summary>
    /// Return the current height of screen.
    /// Shader Only.
    /// </summary>
    public static readonly Float height =
        new("height", ShaderOrigin.Global, [heightDep]);

    /// <summary>
    /// Return the current time of application.
    /// Shader Only.
    /// </summary>
    public static readonly Float t =
        new("t", ShaderOrigin.Global, [timeDep]);

    /// <summary>
    /// Get the parent render polygon.
    /// </summary>
    public static Polygon poly
    {
        get => Empty; // TODO
    }

    /// <summary>
    /// Get or set if the current render is in verbose mode.
    /// </summary>
    public static bool verbose
    {
        get
        {
            var ctx = RenderContext.GetContext();
            return ctx?.Verbose ?? false;
        }
        set
        {
            var ctx = RenderContext.GetContext();
            if (ctx is null)
                return;
            
            ctx.Verbose = value;
        }
    }

    /// <summary>
    /// Reduce many render calls in a unique call.
    /// </summary>
    public static void reduce(Func<int> repeatCount, Action<Float> expression)
    {
        throw new NotImplementedException();
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
    public static readonly Float x = new(
        "pixelPos.x", ShaderOrigin.FragmentShader, [pixelDep, bufferDep]
    );

    /// <summary>
    /// Get the y position of pixel.
    /// </summary>
    public static readonly Float y = new(
        "pixelPos.y", ShaderOrigin.FragmentShader, [pixelDep, bufferDep]
    );

    /// <summary>
    /// Get the z position of pixel.
    /// </summary>
    public static readonly Float z = new(
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
    /// Clean the entire screen.
    /// Shader Only.
    /// </summary>
    public static void clear(Vec4 color)
    {
        var ctx = RenderContext.GetContext()
            ?? throw new ShaderOnlyResourceException();
        ctx.AddClear(color);
    }

    /// <summary>
    /// /// Draw the polygon in the screen.
    /// </summary>
    public static void draw()
    {
        var ctx = RenderContext.GetContext()
            ?? throw new ShaderOnlyResourceException();
        ctx.AddDraw();
    }

    /// <summary>
    /// Fill the polygon in the screen.
    /// </summary>
    public static void fill()
    {
        var ctx = RenderContext.GetContext()
            ?? throw new ShaderOnlyResourceException();
        ctx.AddFill();
    }

    /// <summary>
    /// Fill the direct triangules between the points the polygon in the screen.
    /// Search for GL_TRIANGLE_FAN for more details.
    /// </summary>
    public static void fan()
    {
        var ctx = RenderContext.GetContext()
            ?? throw new ShaderOnlyResourceException();
        ctx.AddFan();
    }

    /// <summary>
    /// Connect and fill adjacente points the points the polygon in the screen.
    /// Search for GL_TRIANGLE_STRIP for more details.
    /// </summary>
    public static void strip()
    {
        var ctx = RenderContext.GetContext()
            ?? throw new ShaderOnlyResourceException();
        ctx.AddStrip();
    }

    /// <summary>
    /// Connect and fill adjacente points the points the polygon in the screen.
    /// Search for GL_LINES for more details.
    /// </summary>
    public static void lines()
    {
        var ctx = RenderContext.GetContext()
            ?? throw new ShaderOnlyResourceException();
        ctx.AddLines();
    }

    /// <summary>
    /// Returns the cosine of the specified angle.
    /// Shader Only.
    /// </summary>
    public static Float cos(Float angle)
        => func<Float>("cos", angle);

    /// <summary>
    /// Returns the sine of the specified angle.
    /// Shader Only.
    /// </summary>
    public static Float sin(Float angle)
        => func<Float>("sin", angle);

    /// <summary>
    /// Returns the tangent of the specified angle.
    /// Shader Only.
    /// </summary>
    public static Float tan(Float angle)
        => func<Float>("tan", angle);

    /// <summary>
    /// Returns the exponential (e^x) of the specified value.
    /// Shader Only.
    /// </summary>
    public static Float exp(Float value)
        => func<Float>("exp", value);

    /// <summary>
    /// Returns the exponential (2^x) of the specified value.
    /// Shader Only.
    /// </summary>
    public static Float exp2(Float value)
        => func<Float>("exp2", value);

    /// <summary>
    /// Returns the logarithm (base e) of the specified value.
    /// Shader Only.
    /// </summary>
    public static Float log(Float value)
        => func<Float>("log", value);

    /// <summary>
    /// Returns the logarithm (base 2) of the specified value.
    /// Shader Only.
    /// </summary>
    public static Float log2(Float value)
        => func<Float>("log2", value);

    /// <summary>
    /// Perform Hermite interpolation between two values.
    /// Shader Only.
    /// </summary>
    public static Float smoothstep(
        Float edge0,
        Float edge1,
        Float x
    )  => func<Float>("smoothstep", edge0, edge1, x);

    /// <summary>
    /// Generate a step function by comparing two values.
    /// Shader Only.
    /// </summary>
    public static Float step(
        Float edge0,
        Float x
    )  => func<Float>("step", edge0, x);
    
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
    public static Float length(Vec2ShaderObject vec) 
        => func<Float>("length", vec);

    /// <summary>
    /// Calculate the length of a vector.
    /// Shader Only.
    /// </summary>
    public static Float length(Vec3ShaderObject vec) 
        => func<Float>("length", vec);

    /// <summary>
    /// Calculate the distance between two points.
    /// Shader Only.
    /// </summary>
    public static Float distance(Vec2ShaderObject p0, Vec2ShaderObject p1)
        => func<Float>("distance", p0, p1);

    /// <summary>
    /// Calculate the distance between two points.
    /// Shader Only.
    /// </summary>
    public static Float distance(Vec3ShaderObject p0, Vec3ShaderObject p1)
        => func<Float>("distance", p0, p1);

    /// <summary>
    /// Calculate the dot product of two vectors.
    /// Shader Only.
    /// </summary>
    public static Float dot(Vec3ShaderObject v0, Vec3ShaderObject v1) 
        => func<Float>("dot", v0, v1);

    /// <summary>
    /// Calculate the dot product of two vectors.
    /// Shader Only.
    /// </summary>
    public static Float dot(Vec2ShaderObject v0, Vec2ShaderObject v1)
        => func<Float>("dot", v0, v1);

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
    public static Float round(Float angle)
        => func<Float>("round", angle);

    /// <summary>
    /// Find the nearest integer less than or equal to the parameter.
    /// Shader Only.
    /// </summary>
    public static Float floor(Float angle)
        => func<Float>("floor", angle);

    /// <summary>
    /// Find the nearest integer that is greater than or equal to the parameter.
    /// Shader Only.
    /// </summary>
    public static Float ceil(Float angle)
        => func<Float>("ceil", angle);

    /// <summary>
    /// Find the truncated value of the parameter.
    /// Shader Only.
    /// </summary>
    public static Float trunc(Float angle)
        => func<Float>("trunc", angle);

    /// <summary>
    /// Return the greater of two values.
    /// Shader Only.
    /// </summary>
    public static Float max(Float x, Float y)
        => func<Float>("max", x, y);
    
    /// <summary>
    /// Return the lesser of two values.
    /// Shader Only.
    /// </summary>
    public static Float min(Float x, Float y)
        => func<Float>("min", x, y);
    
    /// <summary>
    /// Linearly interpolate between two values.
    /// Shader Only.
    /// </summary>
    public static T mix<T>(T x, T y, Float a) 
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
    
    static Float var(Float obj, string name)
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
    
    static Float autoVar(Float obj)
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

    #endregion
}