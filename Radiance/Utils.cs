/* Author:  Leonardo Trevisan Silio
 * Date:    24/12/2024
 */
#pragma warning disable IDE1006

using System;
using System.Text;
using System.Collections.Generic;

namespace Radiance;

using Buffers;
using Shaders;
using Shaders.Dependencies;
using Contexts;
using Primitives;
using Exceptions;
using Animations;

using Float = val;
using Sampler = img;

/// <summary>
/// A facade with all utils to use Radiance shader features.
/// </summary>
public static class Utils
{
    #region PRIMITIVE UTILS

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

    /// <summary>
    /// Use to skip parameters on currying process.
    /// </summary>
    public static readonly SkipCurryingParameter skip = new();

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static Vec2 vec(float x, float y)
        => new(x, y);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static Vec3 vec(float x, float y, float z)
        => new(x, y, z);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static Vec3 vec(Vec2 v, float z)
        => new(v.X, v.Y, z);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static Vec3 vec(float x, Vec2 v)
        => new(x, v.X, v.Y);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static Vec4 vec(float x, float y, float z, float w)
        => new(x, y, z, w);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static Vec4 vec(Vec2 v, float z, float w)
        => new(v.X, v.Y, z, w);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static Vec4 vec(float x, float y, Vec2 v)
        => new(x, y, v.X, v.Y);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static Vec4 vec(float x, Vec2 v, float w)
        => new(x, v.X, v.Y, w);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static Vec4 vec(Vec2 v, Vec2 u)
        => new(v.X, v.Y, u.X, u.Y);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static Vec4 vec(Vec3 v, float w)
        => new(v.X, v.Y, v.Z, w);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static Vec4 vec(float x, Vec3 v)
        => new(x, v.X, v.Y, v.Z);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static vec2 vec(Float x, Float y)
        => (x, y);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static vec3 vec(Float x, Float y, Float z)
        => (x, y, z);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static vec3 vec(vec2 v, Float z)
        => (v.x, v.y, z);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static vec3 vec(Float x, vec2 v)
        => (x, v.x, v.y);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static vec4 vec(Float x, Float y, Float z, Float w)
        => (x, y, z, w);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static vec4 vec(vec2 v, Float z, Float w)
        => (v.x, v.y, z, w);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static vec4 vec(Float x, Float y, vec2 v)
        => (x, y, v.x, v.y);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static vec4 vec(Float x, vec2 v, Float w)
        => (x, v.x, v.y, w);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static vec4 vec(vec2 v, vec2 u)
        => (v.x, v.y, u.x, u.y);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static vec4 vec(vec3 v, Float w)
        => (v.x, v.y, v.z, w);

    /// <summary>
    /// Create a vector.
    /// </summary>
    public static vec4 vec(Float x, vec3 v)
        => (x, v.x, v.y, v.z);

    public static readonly Vec4 red = new(1, 0, 0, 1);
    public static readonly Vec4 green = new(0, 1, 0, 1);
    public static readonly Vec4 blue = new(0, 0, 1, 1);
    public static readonly Vec4 yellow = new(1, 1, 0, 1);
    public static readonly Vec4 black = new(0, 0, 0, 1);
    public static readonly Vec4 white = new(1, 1, 1, 1);
    public static readonly Vec4 cyan = new(0, 1, 1, 1);
    public static readonly Vec4 magenta = new(1, 0, 1, 1);
    public static readonly Vec4 trasnparent = new(0, 0, 0, 0);

    #endregion

    #region BUFFER UTILS

    static BufferedDataArray fillBuffer<T>(int rows, int columns, Func<int, T> factory)
        where T : IBufferizable
    {
        List<BufferData> streams = [];

        for (int i = 0; i < columns; i++)
        {
            var stream = new BufferData(1, 1, false);
            streams.Add(stream);
            stream.PrepareSize(rows);
        }

        var buffer = new float[columns];
        for (int i = 0; i < rows; i++)
        {
            var bufferizable = factory(i);
            bufferizable.Bufferize(buffer, 0);

            for (int j = 0; j < columns; j++)
                streams[j].Add(buffer[j]);
        }

        return new BufferedDataArray(streams);
    }

    /// <summary>
    /// Create a buffer based on a function.
    /// </summary>
    public static BufferData buffer(int size, Func<int, float> factory)
    {
        var stream = new BufferData(1, 1, false);

        stream.PrepareSize(size);
        for (int i = 0; i < size; i++)
            stream.Add(factory(i));

        return stream;
    }

    /// <summary>
    /// Create a buffer based on a function.
    /// </summary>
    public static BufferedDataArray buffer(int size, Func<int, Vec2> factory)
        => fillBuffer(size, 2, factory);

    /// <summary>
    /// Create a buffer based on a function.
    /// </summary>
    public static BufferedDataArray buffer(int size, Func<int, Vec3> factory)
        => fillBuffer(size, 3, factory);

    /// <summary>
    /// Create a buffer based on a function.
    /// </summary>
    public static BufferedDataArray buffer(int size, Func<int, Vec4> factory)
        => fillBuffer(size, 4, factory);

    /// <summary>
    /// Create a buffer from a array.
    /// </summary>
    public static BufferData buffer(params float[] data)
    {
        var stream = new BufferData(1, 1, false);

        stream.PrepareSize(data.Length);
        stream.AddRange(data);

        return stream;
    }
        
    /// <summary>
    /// Get factories for use to create buffers.
    /// </summary>
    public static readonly Factories factories = new();
    public class Factories
    {
        /// <summary>
        /// factory for rand values. Uniform between 0 and 1.
        /// </summary>
        public Func<int, float> urand => rand(0, 1f);

        /// <summary>
        /// factory for rand values. Uniform between 0 and 1.
        /// </summary>
        public Func<int, Vec2> urand2 => rand2(0, 1f);

        /// <summary>
        /// factory for rand values. Uniform between 0 and 1.
        /// </summary>
        public Func<int, Vec3> urand3 => rand3(0, 1f);

        /// <summary>
        /// factory for rand values. Uniform between 0 and 1.
        /// </summary>
        public Func<int, Vec4> urand4 => rand4(0, 1f);

        /// <summary>
        /// factory for rand values.
        /// </summary>
        /// <param name="max">Max value generated.</param>
        /// <param name="min">Min value generated.</param>
        /// <param name="seed">The seed of random algorithm. If null create a seed based on time.</param>
        public Func<int, float> rand(float min, float max, int? seed = null)
        {
            var rand = getRand(min, max, seed);
            return _ => rand();
        }

        /// <summary>
        /// factory for rand values on vec2(x, y) format.
        /// </summary>
        /// <param name="max">Max value generated.</param>
        /// <param name="min">Min value generated.</param>
        /// <param name="seed">The seed of random algorithm. If null create a seed based on time.</param>
        public Func<int, Vec2> rand2(float min, float max, int? seed = null)
        {
            var rand = getRand(min, max, seed);
            return _ => (rand(), rand());
        }

        /// <summary>
        /// factory for rand values on vec3(x, y, z) format.
        /// </summary>
        /// <param name="max">Max value generated.</param>
        /// <param name="min">Min value generated.</param>
        /// <param name="seed">The seed of random algorithm. If null create a seed based on time.</param>
        public Func<int, Vec3> rand3(float min, float max, int? seed = null)
        {
            var rand = getRand(min, max, seed);
            return _ => (rand(), rand(), rand());
        }

        /// <summary>
        /// factory for rand values on vec3(x, y, z) format.
        /// </summary>
        /// <param name="max">Max value generated.</param>
        /// <param name="min">Min value generated.</param>
        /// <param name="seed">The seed of random algorithm. If null create a seed based on time.</param>
        public Func<int, Vec4> rand4(float min, float max, int? seed = null)
        {
            var rand = getRand(min, max, seed);
            return _ => (rand(), rand(), rand(), rand());
        }

        /// <summary>
        /// Build a function that generates random values between min and max using a seed.
        /// </summary>
        private Func<float> getRand(float min, float max, int? seed)
        {
            seed ??= (int)(DateTime.UtcNow.Ticks % int.MaxValue);
            var random = new Random(seed.Value);
            var band = max - min;

            return () => band * random.NextSingle() + min;
        }

        /// <summary>
        /// Generate a repetitive sequence of values.
        /// </summary>
        public Func<int, float> mod(params float[] values)
            => i => values[i % values.Length];
            
        /// <summary>
        /// Generate a repetitive sequence of values.
        /// </summary>
        public Func<int, T> mod<T>(params T[] values)
            where T : IBufferizable
            => i => values[i % values.Length];
    }

    #endregion

    #region RENDER UTILS

    /// <summary>
    /// Create render based on received function.
    /// </summary>
    public static dynamic render(
        Action function)
        => renderDelegate(function);
    
    /// <summary>
    /// Create render based on received function.
    /// </summary>
    public static dynamic render<T1>(
        Action<T1> function)
        // where T1 : IAlias
        => renderDelegate(function);
    
    /// <summary>
    /// Create render based on received function.
    /// </summary>
    public static dynamic render<T1, T2>(
        Action<T1, T2> function)
        // where T1 : IAlias where T2 : IAlias
        => renderDelegate(function);
    
    /// <summary>
    /// Create render based on received function.
    /// </summary>
    public static dynamic render<T1, T2, T3>(
        Action<T1, T2, T3> function)
        // where T1 : IAlias where T2 : IAlias where T3 : IAlias
        => renderDelegate(function);
    
    /// <summary>
    /// Create render based on received function.
    /// </summary>
    public static dynamic render<T1, T2, T3, T4>(
        Action<T1, T2, T3, T4> function)
        // where T1 : IAlias where T2 : IAlias where T3 : IAlias where T4 : IAlias
        => renderDelegate(function);
    
    /// <summary>
    /// Create render based on received function.
    /// </summary>
    public static dynamic render<T1, T2, T3, T4, T5>(
        Action<T1, T2, T3, T4, T5> function)
        // where T1 : IAlias where T2 : IAlias where T3 : IAlias where T4 : IAlias
        // where T5 : IAlias
        => renderDelegate(function);
    
    /// <summary>
    /// Create render based on received function.
    /// </summary>
    public static dynamic render<T1, T2, T3, T4, T5, T6>(
        Action<T1, T2, T3, T4, T5, T6> function)
        // where T1 : IAlias where T2 : IAlias where T3 : IAlias where T4 : IAlias
        // where T5 : IAlias where T6 : IAlias
        => renderDelegate(function);

    private static dynamic renderDelegate(Delegate function)
    {
        ArgumentNullException.ThrowIfNull(function, nameof(function));
        return new Render(function);
    }

    #endregion

    #region BUILT-IN RENDERS

    private static dynamic? moveRender;
    private static void initMoveRender()
    {
        moveRender ??= render((vec3 delta) => {
            var moveValue = autoVar(pos + delta);
            pos = moveValue;
        });
    }
    
    /// <summary>
    /// Move the polygon by a (x, y) vector.
    /// This render cannot perform draw/fill, consider using inside another shader.
    /// </summary>
    public static void move(Float x, Float y)
    {
        initMoveRender();

        moveRender(vec(x, y, 0));
    }
    
    /// <summary>
    /// Move the polygon by a (x, y) vector.
    /// This render cannot perform draw/fill, consider using inside another shader.
    /// </summary>
    public static void move(vec2 vec2)
    {
        initMoveRender();

        moveRender(vec(vec2.x, vec2.y, 0));
    }
    
    /// <summary>
    /// Move the polygon by a (x, y, z) vector.
    /// This render cannot perform draw/fill, consider using inside another shader.
    /// </summary>
    public static void move(Float x, Float y, Float z)
    {
        initMoveRender();

        moveRender(vec(x, y, z));
    }
    
    /// <summary>
    /// Move the polygon by a (x, y, z) vector.
    /// This render cannot perform draw/fill, consider using inside another shader.
    /// </summary>
    public static void move(vec3 vec)
    {
        initMoveRender();

        moveRender(vec);
    }

    private static dynamic? centralizeRender;
    /// <summary>
    /// Centralize a polygon on the center of the screen.
    /// This render cannot perform draw/fill, consider using inside another shader.
    /// </summary>
    public static void centralize()
    {
        centralizeRender ??= render(() => {
            var centerValue = autoVar(pos + (width / 2, height / 2, 0));
            pos = centerValue;
        });

        centralizeRender();
    }

    private static dynamic? zoomRender;
    /// <summary>
    /// Receiving x, y and a factor, performa a zoom on polygon on point (x, y) with the factor scale.
    /// This render cannot perform draw/fill, consider using inside another shader.
    /// </summary>
    public static void zoom(Float x, Float y, Float factor)
    {
        zoomRender ??= render((Float cx, Float cy, Float factor) => {
            var cxValue = autoVar(cx);
            var cyValue = autoVar(cy);
            var factorValue = autoVar(factor);

            var nx = factorValue * (pos.x - cxValue) + cxValue;
            var ny = factorValue * (pos.y - cyValue) + cyValue;
            var zoomValue = autoVar((nx, ny, pos.z));
            
            pos = zoomValue;
        });

        zoomRender(x, y, factor);
    }
    
    private static dynamic? originZoomRender;
    /// <summary>
    /// Receiving a factor, performa a zoom on polygon on point (x, y) with the factor scale.
    /// This render cannot perform draw/fill, consider using inside another shader.
    /// </summary>
    public static void zoom(Float factor)
    {
        originZoomRender ??= render((Float factor) => {
            var factorValue = autoVar(factor);

            var nx = factorValue * pos.x;
            var ny = factorValue * pos.y;
            var zoomValue = autoVar((nx, ny, pos.z));
            
            pos = zoomValue;
        });

        originZoomRender(factor);
    }
    
    private static dynamic? rotateRender;
    /// <summary>
    /// Rotate the polygon a specific angle.
    /// </summary>
    public static void rotate(Float angle)
    {
        rotateRender ??= render((Float angle) => {
            var paramValue = autoVar(angle);
            var cosValue = autoVar(cos(paramValue));
            var sinValue = autoVar(sin(paramValue));
            var rotateValue = autoVar((
                pos.x * cosValue - pos.y * sinValue,
                pos.y * cosValue + pos.x * sinValue,
                pos.z
            ));
            pos = rotateValue;
        });

        rotateRender(angle);
    }

    #endregion

    #region SHADER ONLY UTILS

    /// <summary>
    /// Return the current width of screen.
    /// Shader Only.
    /// </summary>
    public static readonly Float width =
        new("width", ShaderOrigin.Global, [ShaderDependence.WidthDep]);

    /// <summary>
    /// Return the current height of screen.
    /// Shader Only.
    /// </summary>
    public static readonly Float height =
        new("height", ShaderOrigin.Global, [ShaderDependence.HeightDep]);

    /// <summary>
    /// Return the current time of application.
    /// Shader Only.
    /// </summary>
    public static readonly Float t =
        new("t", ShaderOrigin.Global, [ShaderDependence.TimeDep]);
    
    /// <summary>
    /// Get the id of current processed vertex.
    /// Shader Only.
    /// </summary>
    public static readonly Float id = 
        new("gl_VertexID", ShaderOrigin.VertexShader, []);

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
    /// Get ou update the actual position of a generic point of the drawed polygon.
    /// Shader Only.
    /// </summary>
    public static vec3 pos
    {
        get
        {
            var ctx = RenderContext.GetContext()
                ?? throw new ShaderOnlyResourceException(nameof(pos));
            return ctx.Position;
        }
        set
        {
            var ctx = RenderContext.GetContext()
                ?? throw new ShaderOnlyResourceException(nameof(pos));
            ctx.Position = value;
        }
    }

    /// <summary>
    /// Get the x position of pixel.
    /// </summary>
    public static readonly Float x = new(
        "pixelPos.x", ShaderOrigin.FragmentShader, [ShaderDependence.PixelDep, ShaderDependence.BufferDep]
    );

    /// <summary>
    /// Get the y position of pixel.
    /// </summary>
    public static readonly Float y = new(
        "pixelPos.y", ShaderOrigin.FragmentShader, [ShaderDependence.PixelDep, ShaderDependence.BufferDep]
    );

    /// <summary>
    /// Get the z position of pixel.
    /// </summary>
    public static readonly Float z = new(
        "pixelPos.z", ShaderOrigin.FragmentShader, [ShaderDependence.PixelDep, ShaderDependence.BufferDep]
    );

    /// <summary>
    /// Get ou update the actual color of a generic point inside drawed area.
    /// Shader Only.
    /// </summary>
    public static vec4 color
    {
        get
        {
            var ctx = RenderContext.GetContext()
                ?? throw new ShaderOnlyResourceException(nameof(color));
            return ctx.Color;
        }
        set
        {
            var ctx = RenderContext.GetContext()
                ?? throw new ShaderOnlyResourceException(nameof(color));

            var fragmentAccess = ShaderObject.MergeOrigin(value, ShaderOrigin.FragmentShader);

            var variable = new VariableDependence(fragmentAccess);
            ctx.Color = new vec4(
                variable.Name, ShaderOrigin.FragmentShader,
                [ ..fragmentAccess.Dependencies, variable ]
            );
        }
    }

    /// <summary>
    /// Plot points of the polygon on screen.
    /// </summary>
    public static void plot(float size = 1f)
    {
        var ctx = RenderContext.GetContext()
            ?? throw new ShaderOnlyResourceException(nameof(plot));
        ctx.AddPlot(size);
    }

    /// <summary>
    /// Draw the polygon in the screen.
    /// </summary>
    public static void draw(float width = 1f)
    {
        var ctx = RenderContext.GetContext()
            ?? throw new ShaderOnlyResourceException(nameof(draw));
        ctx.AddDraw(width);
    }

    /// <summary>
    /// Fill the polygon in the screen.
    /// </summary>
    public static void fill()
    {
        var ctx = RenderContext.GetContext()
            ?? throw new ShaderOnlyResourceException(nameof(fill));
        ctx.AddFill();
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
    /// Returns the square root of the specified value.
    /// Shader Only.
    /// </summary>
    public static Float sqrt(Float value)
        => func<Float>("sqrt", value);

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
    /// Returns the value of x modulo y. This is computed as x - y * floor(x/y).
    /// Shader Only.
    /// </summary>
    public static Float mod(Float a, Float b)
        => autoVar(func<Float>("mod", a, b));

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
    public static vec2 step(
        vec2 edge0,
        vec2 x
    )  => func<vec2>("step", edge0, x);
    
    /// <summary>
    /// Generate a step function by comparing two values.
    /// Shader Only.
    /// </summary>
    public static vec3 step(
        vec3 edge0,
        vec3 x
    )  => func<vec3>("step", edge0, x);
    
    /// <summary>
    /// Calculate the length of a vector.
    /// Shader Only.
    /// </summary>
    public static Float length(vec2 vec) 
        => autoVar(func<Float>("length", vec));

    /// <summary>
    /// Calculate the length of a vector.
    /// Shader Only.
    /// </summary>
    public static Float length(vec3 vec) 
        => autoVar(func<Float>("length", vec));

    /// <summary>
    /// Calculate the distance between two points.
    /// Shader Only.
    /// </summary>
    public static Float distance(vec2 p0, vec2 p1)
        => autoVar(func<Float>("distance", p0, p1));

    /// <summary>
    /// Calculate the distance between two points.
    /// Shader Only.
    /// </summary>
    public static Float distance(vec3 p0, vec3 p1)
        => autoVar(func<Float>("distance", p0, p1));

    /// <summary>
    /// Calculate the dot product of two vectors.
    /// Shader Only.
    /// </summary>
    public static Float dot(vec3 v0, vec3 v1) 
        => func<Float>("dot", v0, v1);

    /// <summary>
    /// Calculate the dot product of two vectors.
    /// Shader Only.
    /// </summary>
    public static Float dot(vec2 v0, vec2 v1)
        => func<Float>("dot", v0, v1);

    /// <summary>
    /// Calculate the cross product of two vectors.
    /// Shader Only.
    /// </summary>
    public static vec3 cross(vec3 v0, vec3 v1) 
        => func<vec3>("cross", v0, v1);
    
    /// <summary>
    /// Find the nearest integer to the parameter.
    /// Shader Only.
    /// </summary>
    public static Float round(Float value)
        => func<Float>("round", value);

    /// <summary>
    /// Find the nearest integer less than or equal to the parameter.
    /// Shader Only.
    /// </summary>
    public static Float floor(Float value)
        => func<Float>("floor", value);

    /// <summary>
    /// Find the nearest integer that is greater than or equal to the parameter.
    /// Shader Only.
    /// </summary>
    public static Float ceil(Float value)
        => func<Float>("ceil", value);

    /// <summary>
    /// Find the truncated value of the parameter.
    /// Shader Only.
    /// </summary>
    public static Float trunc(Float value)
        => func<Float>("trunc", value);

    /// <summary>
    /// Find the truncated value of the parameter.
    /// Shader Only.
    /// </summary>
    public static Float fract(Float value)
        => func<Float>("fract", value);

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
    /// Return a random value based ona point.
    /// Source: @patriciogv on https://thebookofshaders.com/13, 2015
    /// Shader Only.
    /// </summary>
    public static Float rand(vec2 point)
        => autoVar(func<Float>("rand", point), ShaderDependence.RandDep);
    
    /// <summary>
    /// Return a noise for a point.
    /// Source: @patriciogv on https://thebookofshaders.com/13, 2015
    /// Shader Only.
    /// </summary>
    public static Float noise(vec2 point)
        => autoVar(func<Float>("noise", point), ShaderDependence.NoiseDep);
    
    /// <summary>
    /// Return the Fractal Brownian Motion.
    /// Source: @patriciogv on https://thebookofshaders.com/13, 2015
    /// Shader Only.
    /// </summary>
    public static Float brownian(vec2 point)
        => autoVar(func<Float>("fbm", point), ShaderDependence.BrownianDep);
    
    /// <summary>
    /// Linearly interpolate between two values.
    /// Shader Only.
    /// </summary>
    public static vec4 mix(vec4 x, vec4 y, Float a) 
        => func<vec4>("mix", x, y, a);

    /// <summary>
    /// Linearly interpolate between two values.
    /// Shader Only.
    /// </summary>
    public static vec3 mix(vec3 x, vec3 y, Float a) 
        => func<vec3>("mix", x, y, a);
    
    /// <summary>
    /// Linearly interpolate between two values.
    /// Shader Only.
    /// </summary>
    public static vec2 mix(vec2 x, vec2 y, Float a) 
        => func<vec2>("mix", x, y, a);

    /// <summary>
    /// Linearly interpolate between two values.
    /// Shader Only.
    /// </summary>
    public static Float mix(Float x, Float y, Float a) 
        => func<Float>("mix", x, y, a);

    /// <summary>
    /// Get a pixel color of a img in a specific position of a texture.
    /// Shader Only.
    /// </summary>
    public static vec4 texture(Sampler img, Float posX, Float posY)
    {
        var transformatedPos = autoVar((posX / img.width, posY / img.height));
        var pixel = autoVar(func<vec4>("texture", img, transformatedPos));
        return pixel;
    }

    /// <summary>
    /// For radiance to create a intermediate variable to compute this value.
    /// Shader Only.
    /// </summary>
    public static Float autoVar(Float obj, params ShaderDependence[] otherDeps)
    {
        var variable = new VariableDependence(obj);
        return new (variable.Name, obj.Origin, [ ..obj.Dependencies, variable , ..otherDeps]);
    }

    /// <summary>
    /// For radiance to create a intermediate variable to compute this value.
    /// Shader Only.
    /// </summary>
    public static vec2 autoVar(vec2 obj, params ShaderDependence[] otherDeps)
    {
        var variable = new VariableDependence(obj);
        return new (variable.Name, obj.Origin, [ ..obj.Dependencies, variable, ..otherDeps ]);
    }

    /// <summary>
    /// For radiance to create a intermediate variable to compute this value.
    /// Shader Only.
    /// </summary>
    public static vec3 autoVar(vec3 obj, params ShaderDependence[] otherDeps)
    {
        var variable = new VariableDependence(obj);
        return new (variable.Name, obj.Origin, [ ..obj.Dependencies, variable, ..otherDeps ]);
    }

    /// <summary>
    /// For radiance to create a intermediate variable to compute this value.
    /// Shader Only.
    /// </summary>
    public static vec4 autoVar(vec4 obj, params ShaderDependence[] otherDeps)
    {
        var variable = new VariableDependence(obj);
        return new (variable.Name, obj.Origin, [ ..obj.Dependencies, variable, ..otherDeps ]);
    }

    /// <summary>
    /// For radiance to create a intermediate variable to compute this value.
    /// Shader Only.
    /// </summary>
    public static Float var(Float obj, string name)
    {
        var dep = new VariableDependence(
            obj.Type.TypeName, name, obj.Expression
        );
        return new (name, obj.Origin, [ ..obj.Dependencies, dep ]);
    }

    /// <summary>
    /// For radiance to create a intermediate variable to compute this value.
    /// Shader Only.
    /// </summary>
    public static vec2 var(vec2 obj, string name)
    {
        var dep = new VariableDependence(
            obj.Type.TypeName, name, obj.Expression
        );
        return new (name, obj.Origin, [ ..obj.Dependencies, dep ]);
    }

    /// <summary>
    /// For radiance to create a intermediate variable to compute this value.
    /// Shader Only.
    /// </summary>
    public static vec3 var(vec3 obj, string name)
    {
        var dep = new VariableDependence(
            obj.Type.TypeName, name, obj.Expression
        );
        return new (name, obj.Origin, [ ..obj.Dependencies, dep ]);
    }

    /// <summary>
    /// For radiance to create a intermediate variable to compute this value.
    /// Shader Only.
    /// </summary>
    public static vec4 var(vec4 obj, string name)
    {
        var dep = new VariableDependence(
            obj.Type.TypeName, name, obj.Expression
        );
        return new (name, obj.Origin, [ ..obj.Dependencies, dep ]);
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

    #region ANIMATIONS

    /// <summary>
    /// Create a animation.
    /// </summary>
    public static void animation(Action<AnimationBuilder> code)
    {
        var builder = new AnimationBuilder();
        code(builder);
        builder.Build();
    }

    #endregion
}