public static class Utils
{
    internal static Polygon screen = null;
    internal readonly static TimeDependence timeDep = new();
    internal readonly static PixelDependence pixelDep = new();
    internal readonly static BufferDependence bufferDep = new();

    public static bool verbose
    {
        get
        {
            var ctx = GlobalRenderContext.GetContext();
            return ctx.IsVerbose;
        }
        set
        {
            var ctx = GlobalRenderContext.GetContext();
            ctx.IsVerbose = value;
        }
    }
    
    /// <summary>
    /// Clean the entire screen.
    /// Shader Only.
    /// </summary>
    public static void clear(Vec4 color)
    {
        var ctx = GlobalRenderContext.GetContext();
        ctx.AddClear(color);
    }

    /// <summary>
    /// Draw the polygon in the screen.
    /// </summary>
    public static void draw()
    {
        var ctx = GlobalRenderContext.GetContext();
        ctx.AddDraw();
    }

    /// <summary>
    /// Fill the polygon in the screen.
    /// </summary>
    public static void fill()
    {
        var ctx = GlobalRenderContext.GetContext();
        ctx.AddFill();
    }

    /// <summary>
    /// Fill the direct triangules between the points the polygon in the screen.
    /// Search for GL_TRIANGLE_FAN for more details.
    /// </summary>
    public static void fan()
    {
        var ctx = GlobalRenderContext.GetContext();
        ctx.AddFan();
    }

    /// <summary>
    /// Connect and fill adjacente points the points the polygon in the screen.
    /// Search for GL_TRIANGLE_STRIP for more details.
    /// </summary>
    public static void strip()
    {
        var ctx = GlobalRenderContext.GetContext();
        ctx.AddStrip();
    }

    /// <summary>
    /// Connect and fill adjacente points the points the polygon in the screen.
    /// Search for GL_LINES for more details.
    /// </summary>
    public static void lines()
    {
        var ctx = GlobalRenderContext.GetContext();
        ctx.AddLines();
    }

    /// <summary>
    /// Get ou update the actual position of a generic point of the drawed polygon.
    /// Shader Only.
    /// </summary>
    public static Vec3ShaderObject pos
    {
        get
        {
            var ctx = GlobalRenderContext.GetContext();
            return ctx.Position;
        }
        set
        {
            var ctx = GlobalRenderContext.GetContext();
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
            var ctx = GlobalRenderContext.GetContext();
            return ctx.Color;
        }
        set
        {
            var ctx = GlobalRenderContext.GetContext();
            var variable = new VariableDependence(value);
            ctx.Color = new Vec4ShaderObject(
                variable.Name, ShaderOrigin.FragmentShader,
                [..value.Dependencies, variable]
            );
        }
    }
    
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
    /// Return the current center point of screen.
    /// Shader Only.
    /// </summary>
    public static Vec3ShaderObject center => var((width / 2, height / 2, 0), "center");
    

    /// <summary>
    /// Return the current width of screen.
    /// Shader Only.
    /// </summary>
    public static readonly FloatShaderObject width =
        new("width", ShaderOrigin.Global, [widthDep]);

    /// <summary>
    /// Return the current height of screen.
    /// Shader Only.
    /// </summary>
    public static readonly FloatShaderObject height =
        new("height", ShaderOrigin.Global, [heightDep]);

    /// <summary>
    /// Return the current time of application.
    /// Shader Only.
    /// </summary>
    public static readonly FloatShaderObject t =
        new("t", ShaderOrigin.Global, [timeDep]);

    /// <summary>
    /// Return the time when t is zero.
    /// </summary>
    public static DateTime ZeroTime => Clock.ZeroTime;

    /// <summary>
    /// Return the time in seconds of application using the Clock.Shared.
    /// </summary>
    public static float Time => Clock.Shared.Time;
    
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
    /// Get a rectangle with size of opened screen centralizated in center of screen.
    /// </summary>
    public static Polygon Screen
    {
        get
        {
            screen ??= 
                Window.IsOpen ?
                Rect(0, 0, 0, Window.Width, Window.Height).ToImmutable() :
                throw new WindowClosedException();
            
            return screen;
        }
    }

    /// <summary>
    /// Create a rectangle with specific width and height
    /// centralized on (0, 0, 0) cordinate.
    /// </summary>
    public static Polygon Rect(float width, float height)
    {
        var halfWid = width / 2;
        var halfHei = height / 2;
        return [
            -halfWid, -halfHei, 0,
            -halfHei, halfWid, 0,
            halfHei, halfWid, 0,
            halfHei, -halfWid, 0
        ];
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
        return [
            x - halfWid, y - halfHei, z,
            x - halfWid, y + halfHei, z,
            x + halfWid, y + halfHei, z,
            x + halfWid, y - halfHei, z
        ];
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
        Polygon result = new MutablePolygon();

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
        Polygon result = new MutablePolygon();

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
        var result = new MutablePolygon();

        foreach (var v in vectors)
            result.Add(v.X, v.Y, v.Z);
        
        return result;
    }
    
    /// <summary>
    /// Create a polygon based in recived data.
    /// </summary>
    public static Polygon Data(params Vec2[] vectors)
    {
        var result = new MutablePolygon();

        foreach (var v in vectors)
            result.Add(v.X, v.Y, 0);
        
        return result;
    }
    
    /// <summary>
    /// Get a Kit of autoimplemented renders.
    /// </summary>
    public static RenderKit Kit => RenderKit.Shared;    

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
        FloatShaderObject, FloatShaderObject, 
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
        FloatShaderObject, FloatShaderObject, 
        FloatShaderObject, FloatShaderObject,
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
        FloatShaderObject, FloatShaderObject,
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
        FloatShaderObject, FloatShaderObject, 
        FloatShaderObject, FloatShaderObject,
        FloatShaderObject, FloatShaderObject,
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
        FloatShaderObject, FloatShaderObject,
        FloatShaderObject, FloatShaderObject,
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
        FloatShaderObject, Sampler2DShaderObject> function)
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
        FloatShaderObject, FloatShaderObject,
        FloatShaderObject, Sampler2DShaderObject> function)
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
        FloatShaderObject, Sampler2DShaderObject,
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
        FloatShaderObject, FloatShaderObject,
        Sampler2DShaderObject, Sampler2DShaderObject> function)
    {
        if (function is null)
            throw new ArgumentNullException("function");
        
        return new Render(function);
    }
}