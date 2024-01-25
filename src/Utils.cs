/* Author:  Leonardo Trevisan Silio
 * Date:    21/01/2024
 */
using System;
using System.Text;
using System.Linq;

namespace Radiance;

using Data;

using Renders;

using Internal;

using Shaders;
using Shaders.Objects;
using Shaders.Dependencies;

/// <summary>
/// A facade with all utils to use Radiance features.
/// </summary>
public static class Utils
{
    private static int variableCount = 0;

    internal readonly static Polygon square = Rect(1, 1);
    internal readonly static Polygon circle = Ellipse(1, 1, 128);
    internal readonly static TimeDependence _t = new();
    internal readonly static WidthWindowDependence _width = new();
    internal readonly static HeightWindowDependence _height = new();
    
    /// <summary>
    /// Clean the entire screen.
    /// Shader Only.
    /// </summary>
    public static void clear(Vec4 color)
    {
        var ctx = RenderContext.GetContext();
        var man = ctx.Manager;
        man.AddClear(color);
    }

    /// <summary>
    /// Draw the polygon in the screen.
    /// </summary>
    public static void draw()
    {
        var ctx = RenderContext.GetContext();
        var man = ctx.Manager;
        man.AddDraw();
    }

    /// <summary>
    /// Fill the polygon in the screen.
    /// </summary>
    public static void fill()
    {
        var ctx = RenderContext.GetContext();
        var man = ctx.Manager;
        man.AddFill();
    }

    /// <summary>
    /// Get ou update the actual position of a generic point of the drawed polygon.
    /// Shader Only.
    /// </summary>
    public static Vec3ShaderObject pos
    {
        get
        {
            var ctx = RenderContext.GetContext();
            return ctx.Position;
        }
        set
        {
            var ctx = RenderContext.GetContext();
            ctx.Position = value;
        }
    }

    public static FloatShaderObject x
    {
        get
        {
            var ctx = RenderContext.GetContext();
            return ctx.Position.x;
        }
        set
        {

        }
    }

    public static FloatShaderObject y
    {
        get
        {
            var ctx = RenderContext.GetContext();
            return ctx.Position.y;
        }
        set
        {

        }
    }

    public static FloatShaderObject z
    {
        get
        {
            var ctx = RenderContext.GetContext();
            return ctx.Position.z;
        }
        set
        {

        }
    }

    /// <summary>
    /// Get ou update the actual color of a generic point inside drawed area.
    /// Shader Only.
    /// </summary>
    public static Vec4ShaderObject color
    {
        get
        {
            var ctx = RenderContext.GetContext();
            return ctx.Color;
        }
        set
        {
            var ctx = RenderContext.GetContext();
            ctx.Color = value;
        }
    }
    
    /// <summary>
    /// Get the time between two frames.
    /// </summary>
    public static float dt => Window.DeltaTime;

    /// <summary>
    /// Get (1, 0, 0) vector.
    /// </summary>
    public static Vec3 i => new(1, 0, 0); 
    
    /// <summary>
    /// Get (0, 1, 0) vector.
    /// </summary>
    public static Vec3 j => new(0, 1, 0);

    /// <summary>
    /// Get (0, 0, 1) vector.
    /// </summary>
    public static Vec3 k => new(0, 0, 1);

    /// <summary>
    /// Get (0, 0, 0) origin vector.
    /// </summary>
    public static Vec3 origin => new(0, 0, 0);

    public static Vec4 red => new(1, 0, 0, 1);
    public static Vec4 green => new(0, 1, 0, 1);
    public static Vec4 blue => new(0, 0, 1, 1);
    public static Vec4 yellow => new(1, 1, 0, 1);
    public static Vec4 black => new(0, 0, 0, 1);
    public static Vec4 white => new(1, 1, 1, 1);
    public static Vec4 cyan => new(0, 1, 1, 1);
    public static Vec4 magenta => new(1, 0, 1, 1);

    /// <summary>
    /// Return the current center point of screen.
    /// Shader Only.
    /// </summary>
    public static Vec3ShaderObject center => var((width / 2, height / 2, 0), "center");
    

    /// <summary>
    /// Return the current width of screen.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject width => _width;

    /// <summary>
    /// Return the current height of screen.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject height => _height;

    /// <summary>
    /// Return the current time of application.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject t => _t;

    /// <summary>
    /// Return the time when t is zero.
    /// </summary>
    public static DateTime ZeroTime => _t.ZeroTime;

    /// <summary>
    /// Get a circle with radius 1 centralizated in (0, 0, 0)
    /// with 128 sides.
    /// </summary>
    public static Polygon Circle => circle;

    /// <summary>
    /// Get a square with side 1 centralizated in (0, 0, 0).
    /// </summary>
    public static Polygon Square => square;

    /// <summary>
    /// Get a rectangle with size of opened screen centralizated in center of screen.
    /// </summary>
    public static Polygon Screen => Rect(0, 0, 0, Window.Width, Window.Height);

    /// <summary>
    /// Create a rectangle with specific width and height
    /// centralized on (0, 0, 0) cordinate.
    /// </summary>
    public static Polygon Rect(float width, float height)
    {
        var halfWid = width / 2;
        var halfHei = height / 2;
        return new Polygon()
            .Add(-halfWid, -halfHei, 0)
            .Add(-halfHei, halfWid, 0)
            .Add(halfHei, halfWid, 0)
            .Add(halfHei, -halfWid, 0);
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
        return new Polygon()
            .Add(x - halfWid, y - halfHei, z)
            .Add(x - halfHei, y + halfWid, z)
            .Add(x + halfHei, y + halfWid, z)
            .Add(x + halfHei, x - halfWid, z);
    }

    /// <summary>
    /// Create a ellipse with specific a and b radius
    /// centralized on (x, y, z) cordinate with a specific
    /// quantity of sides.
    /// </summary>
    public static Polygon Ellipse(
        float x, float y, float z,
        float a, float b = float.NaN,
        int sizes = 63
    )
    {
        Polygon result = new Polygon();

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
    public static Polygon Ellipse(
        float a, float b = float.NaN,
        int sizes = 63
    )
    {
        Polygon result = new Polygon();

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
    public static Polygon Data(params Vec3[] vectors)
    {
        var result = new Polygon();

        foreach (var v in vectors)
            result.Add(v.X, v.Y, v.Z);
        
        return result;
    }
    
    /// <summary>
    /// Create a polygon based in recived data.
    /// </summary>
    public static Polygon Data(params Vec2[] vectors)
    {
        var result = new Polygon();

        foreach (var v in vectors)
            result.Add(v.X, v.Y, 0);
        
        return result;
    }

    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static dynamic render(Action function)
    {
        if (function is null)
            throw new ArgumentNullException("function");
        
        return new Render(function);
    }

    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static dynamic render(Action<FloatShaderObject> function)
    {
        if (function is null)
            throw new ArgumentNullException("function");
        
        return new Render(function);
    }
    
    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static dynamic render(Action<
        FloatShaderObject, FloatShaderObject> function)
    {
        if (function is null)
            throw new ArgumentNullException("function");
        
        return new Render(function);
    }
    
    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static dynamic render(Action<
        FloatShaderObject, FloatShaderObject,
        FloatShaderObject> function)
    {
        if (function is null)
            throw new ArgumentNullException("function");
        
        return new Render(function);
    }

    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static dynamic render(Action<
        FloatShaderObject, FloatShaderObject, 
        FloatShaderObject, FloatShaderObject> function)
    {
        if (function is null)
            throw new ArgumentNullException("function");
        
        return new Render(function);
    }

    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static dynamic render(Action<
        Sampler2DShaderObject> function)
    {
        if (function is null)
            throw new ArgumentNullException("function");
        
        return new Render(function);
    }

    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static dynamic render(Action<
        Sampler2DShaderObject, FloatShaderObject> function)
    {
        if (function is null)
            throw new ArgumentNullException("function");
        
        return new Render(function);
    }

    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static dynamic render(Action<
        Sampler2DShaderObject, FloatShaderObject,
        FloatShaderObject> function)
    {
        if (function is null)
            throw new ArgumentNullException("function");
        
        return new Render(function);
    }

    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static dynamic render(Action<
        Sampler2DShaderObject, FloatShaderObject,
        FloatShaderObject, FloatShaderObject> function)
    {
        if (function is null)
            throw new ArgumentNullException("function");
        
        return new Render(function);
    }

    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static dynamic render(Action<
        Sampler2DShaderObject, Sampler2DShaderObject> function)
    {
        if (function is null)
            throw new ArgumentNullException("function");
        
        return new Render(function);
    }

    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static dynamic render(Action<
        Sampler2DShaderObject, Sampler2DShaderObject,
        FloatShaderObject> function)
    {
        if (function is null)
            throw new ArgumentNullException("function");
        
        return new Render(function);
    }

    /// <summary>
    /// Create render with shaders based on function recived.
    /// </summary>
    public static dynamic render(Action<
        Sampler2DShaderObject, Sampler2DShaderObject,
        FloatShaderObject, FloatShaderObject> function)
    {
        if (function is null)
            throw new ArgumentNullException("function");
        
        return new Render(function);
    }

    /// <summary>
    /// Returns the cosine of the specified angle.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject cos(FloatShaderObject angle)
        => floatFunc("cos", angle);

    /// <summary>
    /// Returns the sine of the specified angle.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject sin(FloatShaderObject angle)
        => floatFunc("sin", angle);

    /// <summary>
    /// Returns the tangent of the specified angle.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject tan(FloatShaderObject angle)
        => floatFunc("tan", angle);

    /// <summary>
    /// Returns the exponential (e^x) of the specified value.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject exp(FloatShaderObject value)
        => floatFunc("exp", value);

    /// <summary>
    /// Returns the exponential (2^x) of the specified value.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject exp2(FloatShaderObject value)
        => floatFunc("exp2", value);

    /// <summary>
    /// Returns the logarithm (base e) of the specified value.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject log(FloatShaderObject value)
        => floatFunc("log", value);

    /// <summary>
    /// Returns the logarithm (base 2) of the specified value.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject log2(FloatShaderObject value)
        => floatFunc("log2", value);

    /// <summary>
    /// Perform Hermite interpolation between two values.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject smoothstep(
        FloatShaderObject edge0,
        FloatShaderObject edge1,
        FloatShaderObject x
    )  => operation<FloatShaderObject>("smoothstep", edge0, edge1, x);

    /// <summary>
    /// Generate a step function by comparing two values.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject step(
        FloatShaderObject edge0,
        FloatShaderObject x
    )  => func<FloatShaderObject, FloatShaderObject, FloatShaderObject>("step", edge0, x);
    
    /// <summary>
    /// Generate a step function by comparing two values.
    /// Shader Only.
    /// </summary>
    public static Vec2ShaderObject step(
        Vec2ShaderObject edge0,
        Vec2ShaderObject x
    )  => func<Vec2ShaderObject, Vec2ShaderObject, Vec2ShaderObject>("step", edge0, x);
    
    /// <summary>
    /// Generate a step function by comparing two values.
    /// Shader Only.
    /// </summary>
    public static Vec3ShaderObject step(
        Vec3ShaderObject edge0,
        Vec3ShaderObject x
    )  => func<Vec3ShaderObject, Vec3ShaderObject, Vec3ShaderObject>("step", edge0, x);
    
    /// <summary>
    /// Calculate the length of a vector.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject length(Vec2ShaderObject vec) 
        => func<FloatShaderObject, Vec2ShaderObject>("length", vec);

    /// <summary>
    /// Calculate the length of a vector.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject length(Vec3ShaderObject vec) 
        => func<FloatShaderObject, Vec3ShaderObject>("length", vec);

    /// <summary>
    /// Calculate the distance between two points.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject distance(Vec2ShaderObject p0, Vec2ShaderObject p1)
        => operation<Vec2ShaderObject>("distance", p0, p1);

    /// <summary>
    /// Calculate the distance between two points.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject distance(Vec3ShaderObject p0, Vec3ShaderObject p1)
        => operation<Vec3ShaderObject>("distance", p0, p1);

    /// <summary>
    /// Calculate the dot product of two vectors.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject dot(Vec3ShaderObject v0, Vec3ShaderObject v1) 
        => operation<Vec3ShaderObject>("dot", v0, v1);

    /// <summary>
    /// Calculate the dot product of two vectors.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject dot(Vec2ShaderObject v0, Vec2ShaderObject v1)
        => operation<Vec2ShaderObject>("dot", v0, v1);

    /// <summary>
    /// Calculate the cross product of two vectors.
    /// Shader Only.
    /// </summary>
    public static Vec3ShaderObject cross(Vec3ShaderObject v0, Vec3ShaderObject v1) 
        => func<Vec3ShaderObject, Vec3ShaderObject, Vec3ShaderObject>("cross", v0, v1);
    
    /// <summary>
    /// Find the nearest integer to the parameter.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject round(FloatShaderObject angle)
        => floatFunc("round", angle);

    /// <summary>
    /// Find the nearest integer less than or equal to the parameter.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject floor(FloatShaderObject angle)
        => floatFunc("floor", angle);

    /// <summary>
    /// Find the nearest integer that is greater than or equal to the parameter.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject ceil(FloatShaderObject angle)
        => floatFunc("ceil", angle);

    /// <summary>
    /// Find the truncated value of the parameter.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject trunc(FloatShaderObject angle)
        => floatFunc("trunc", angle);

    /// <summary>
    /// Return the greater of two values.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject max(FloatShaderObject x, FloatShaderObject y)
        => func<
            FloatShaderObject,
            FloatShaderObject,
            FloatShaderObject
            >("max", x, y);
    
    /// <summary>
    /// Return the lesser of two values.
    /// Shader Only.
    /// </summary>
    public static FloatShaderObject min(FloatShaderObject x, FloatShaderObject y)
        => func<
            FloatShaderObject,
            FloatShaderObject,
            FloatShaderObject
            >("min", x, y);
    
    /// <summary>
    /// Linearly interpolate between two values.
    /// Shader Only.
    /// </summary>
    public static T mix<T>(T x, T y, FloatShaderObject a) 
        where T : ShaderObject, new()
        => func<T, T, T, FloatShaderObject>("mix", x, y, a);

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
        => autoVar(func<Vec4ShaderObject, Sampler2DShaderObject, Vec2ShaderObject>("texture", img, pos), "tex");
    
    private static FloatShaderObject var(FloatShaderObject obj, string name)
    {
        var dependence = new CodeDependence(
            obj, name
        );
        var variable = new FloatShaderObject(
            name, obj.Dependecies.Append(dependence)
        );
        return variable;
    }

    private static Vec2ShaderObject var(Vec2ShaderObject obj, string name)
    {
        var dependence = new CodeDependence(
            obj, name
        );
        var variable = new Vec2ShaderObject(
            name, obj.Dependecies.Append(dependence)
        );
        return variable;
    }

    private static Vec3ShaderObject var(Vec3ShaderObject obj, string name)
    {
        var dependence = new CodeDependence(
            obj, name
        );
        var variable = new Vec3ShaderObject(
            name, obj.Dependecies.Append(dependence)
        );
        return variable;
    }

    private static Vec4ShaderObject var(Vec4ShaderObject obj, string name)
    {
        var dependence = new CodeDependence(
            obj, name
        );
        var variable = new Vec4ShaderObject(
            name, obj.Dependecies.Append(dependence)
        );
        return variable;
    }
    
    private static FloatShaderObject autoVar(FloatShaderObject obj, string name)
        => var(obj, name + variableCount++);

    private static Vec2ShaderObject autoVar(Vec2ShaderObject obj, string name)
        => var(obj, name + variableCount++);

    private static Vec3ShaderObject autoVar(Vec3ShaderObject obj, string name)
        => var(obj, name + variableCount++);

    private static Vec4ShaderObject autoVar(Vec4ShaderObject obj, string name)
        => var(obj, name + variableCount++);

    private static FloatShaderObject operation<T>(string name, T obj1, T obj2)
        where T : ShaderObject
        => func<FloatShaderObject, T, T>(name, obj1, obj2);

    private static FloatShaderObject operation<T>(string name, T obj1, T obj2, T obj3)
        where T : ShaderObject
        => func<FloatShaderObject, T, T, T>(name, obj1, obj2, obj3);

    private static FloatShaderObject floatFunc(string name, FloatShaderObject input)
        => func<FloatShaderObject, FloatShaderObject>(name, input);

    private static R func<R, P1>(
        string name, 
        P1 obj1
    )
        where R : ShaderObject, new()
        where P1 : ShaderObject
    {
        var result = new R();

        result.Dependecies = obj1.Dependecies;
        
        result.Expression = 
            buildObject(name, obj1);

        return result;
    }

    private static R func<R, P1, P2>(
        string name, 
        P1 obj1, P2 obj2
    )
        where R : ShaderObject, new()
        where P1 : ShaderObject
        where P2 : ShaderObject
    {
        var result = new R();

        result.Dependecies = 
            obj1.Dependecies
            .Concat(obj2.Dependecies);
        
        result.Expression = 
            buildObject(name, obj1, obj2);

        return result;
    }

    private static R func<R, P1, P2, P3>(
        string name, 
        P1 obj1, P2 obj2, P3 obj3
    )
        where R : ShaderObject, new()
        where P1 : ShaderObject
        where P2 : ShaderObject
        where P3 : ShaderObject
    {
        var result = new R();

        result.Dependecies = 
            obj1.Dependecies
            .Concat(obj2.Dependecies)
            .Concat(obj3.Dependecies);
        
        result.Expression = 
            buildObject(name, obj1, obj2, obj3);

        return result;
    }

    private static string buildObject(
        string funcName,
        params object[] inputs
    )
    {
        var sb = new StringBuilder();
        sb.Append($"{funcName}(");

        for (int i = 0; i < inputs.Length - 1; i++)
        {
            if (inputs[i] is ShaderObject input)
                sb.Append(input.Expression);
            sb.Append(", ");
        }

        if (inputs.Length > 0)
        {
            if (inputs[^1] is ShaderObject input)
                sb.Append(input.Expression);
            sb.Append(")");
        }
            
        return sb.ToString();
    }
}