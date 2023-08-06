/* Author:  Leonardo Trevisan Silio
 * Date:    06/08/2023
 */
using System;
using System.Linq;
using System.Threading;

namespace DuckGL;

using ShaderSupport;

/// <summary>
/// A facede with all utils to program a shader.
/// </summary>
public static class Shaders
{
    static ThreadLocal<ShaderContext> ctx;
    static Shaders()
    {
        ctx = new ThreadLocal<ShaderContext>(
            () => new ShaderContext()
        );

        defaultVertex =
        """
        #version 330 core
        layout (location = 0) in vec3 data0;

        void main()
        {
            gl_Position = vec4(data0, 1.0);
        }
        """;

        defaultFragment =
        """
        #version 330 core
        out vec4 FragColor;

        uniform vec4 uniform0;

        void main()
        {
            FragColor = uniform0;
        }
        """;
    }

    internal static void ResetContext()
        => ctx.Value = new ShaderContext();

    internal static ShaderContext GetContext()
        => ctx.Value;
    
    public static readonly string defaultVertex;

    public static readonly string defaultFragment;

    public static ShaderType single
        => ShaderType.Float;

    public static ShaderType vec2
        => ShaderType.Vec2;

    public static ShaderType vec3
        => ShaderType.Vec3;
    
    public static ShaderType vec4
        => ShaderType.Vec4;

    public static ShaderObject vec(
        params ShaderObject[] objs
    )
    {
        int vecGrade = 0;
        for (int i = 0; i < objs.Length; i++)
        {
            var obj = objs[i];
            vecGrade += (int)obj.Type;
        }
        if (vecGrade < 2 || vecGrade > 4)
            throw new Exception("A vec need have a dimension number betwen 2 and 4.");
        
        var exp = $"vec{vecGrade}(";
        for (int i = 0; i < objs.Length; i++)
        {
            var obj = objs[i];
            exp += obj.Value;
            exp += i < objs.Length - 1 ? "," : ")";
        }

        return new ShaderObject(
            (ShaderType)vecGrade,
            null,
            exp
        );
    }

    private static string[] validVersions = {
        "110", "120", "130", "140", "150",
        "330", 
        "400", "410", "420", "430", "440", "450"
    };
    public static void version(string version)
    {
        var parts = version.Split(" ");

        if (parts.Length == 0)
            throw new Exception("Invalid empty version.");
        
        var number = parts[0];
        if (!validVersions.Contains(number))
        {
            string error = "Invalid version of GLSL.\n\tValid versions: ";
            foreach (var valVer in validVersions)
                error += valVer + ", ";
            throw new Exception(error);
        }

        var profile =
            parts.Length > 1 ?
            parts[1] :
            "core";
        if (profile != "core" && profile != "compatibility")
        {
            throw new Exception($"Invalid profile {profile}");
        }

        if (parts.Length > 2)
            throw new Exception("Invalid long version.");

        ctx.Value.Version = version;
    }

    public static void layout(
        int pos,
        ShaderType type,
        out ShaderObject obj)
    {
        obj = new ShaderObject(
            type,
            $"data{pos}"
        );
        ctx.Value.Layout.Add((pos, type));
    }

    public static void outVar(
        ShaderType type,
        string name, 
        ShaderObject data
    )
    {
        if (type != data.Type)
            throw new Exception(
                "Invalid set of out variable with diferent type."
            );

        var obj = new ShaderObject(
            type,
            name
        );

        ctx.Value.OutVariables.Add((obj, data));
    }

    public static void inVar(
        ShaderType type,
        string name,
        out ShaderObject obj
    )
    {
        obj = new ShaderObject(
            type,
            name
        );

        ctx.Value.InVariables.Add(obj);
    }
    
    public static ShaderObject cos(ShaderObject angle)
        => func(ShaderType.Float, "cos", (angle, ShaderType.Float));

    public static ShaderObject sin(ShaderObject angle)
        => func(ShaderType.Float, "sin", (angle, ShaderType.Float));

    public static ShaderObject tan(ShaderObject angle)
        => func(ShaderType.Float, "tan", (angle, ShaderType.Float));

    public static ShaderObject exp(ShaderObject angle)
        => func(ShaderType.Float, "exp", (angle, ShaderType.Float));

    public static ShaderObject exp2(ShaderObject angle)
        => func(ShaderType.Float, "exp2", (angle, ShaderType.Float));

    public static ShaderObject log(ShaderObject angle)
        => func(ShaderType.Float, "log", (angle, ShaderType.Float));

    public static ShaderObject log2(ShaderObject angle)
        => func(ShaderType.Float, "log2", (angle, ShaderType.Float));

    public static ShaderObject smoothstep(
        ShaderObject edge0,
        ShaderObject edge1,
        ShaderObject x
    ) => func(ShaderType.Float, "smoothstep", 
        (edge0, ShaderType.Float),
        (edge1, ShaderType.Float),
        (x, ShaderType.Float)
    );

    public static ShaderObject smootherstep(
        ShaderObject edge0,
        ShaderObject edge1,
        ShaderObject x
    ) => func(ShaderType.Float, "smootherstep", 
        (edge0, ShaderType.Float),
        (edge1, ShaderType.Float),
        (x, ShaderType.Float)
    );

    public static ShaderObject length(ShaderObject vec) 
        => func(ShaderType.Float, "length", 
        (vec, ShaderType.Vec2 | ShaderType.Vec3)
    );

    public static ShaderObject distance(ShaderObject p0, ShaderObject p1) 
        => func(ShaderType.Float, "distance", 
        (p0, ShaderType.Vec2 | ShaderType.Vec3),
        (p1, ShaderType.Vec2 | ShaderType.Vec3)
    );

    public static ShaderObject dot(ShaderObject v0, ShaderObject v1) 
        => func(ShaderType.Float, "dot", 
        (v0, ShaderType.Vec2 | ShaderType.Vec3),
        (v1, ShaderType.Vec2 | ShaderType.Vec3)
    );

    public static ShaderObject cross(ShaderObject v0, ShaderObject v1) 
        => func(ShaderType.Vec3, "cross", 
        (v0, ShaderType.Vec3),
        (v1, ShaderType.Vec3)
    );
    
    public static ShaderObject round(ShaderObject angle)
        => func(ShaderType.Float, "round", (angle, ShaderType.Float));

    public static ShaderObject floor(ShaderObject angle)
        => func(ShaderType.Float, "floor", (angle, ShaderType.Float));

    public static ShaderObject ceil(ShaderObject angle)
        => func(ShaderType.Float, "ceil", (angle, ShaderType.Float));

    public static ShaderObject trunc(ShaderObject angle)
        => func(ShaderType.Float, "trunc", (angle, ShaderType.Float));

    public static ShaderObject max(ShaderObject x, ShaderObject y)
        => func(ShaderType.Float, "max", 
            (x, ShaderType.Float),
            (y, ShaderType.Float)
        );
    
    public static ShaderObject min(ShaderObject x, ShaderObject y)
        => func(ShaderType.Float, "min", 
            (x, ShaderType.Float),
            (y, ShaderType.Float)
        );
    
    public static ShaderObject mix(ShaderObject x, ShaderObject y, ShaderObject a)
        => func(x.Type, "mix", 
            (x, ShaderType.Vec),
            (y, ShaderType.Vec),
            (a, ShaderType.Float)
        );

    public static ShaderObject gl_Position
    {
        get => ctx.Value.Position;
        set
        {
            if (value.Type != ShaderType.Vec4)
                throw new Exception("Invalid type for gl_Position");
            
            ctx.Value.Position = value;
        }
    }

    public static ShaderObject gl_FragColor
        {
        get => ctx.Value.FragColor;
        set
        {
            if (value.Type != ShaderType.Vec4)
                throw new Exception("Invalid type for gl_FragColor");
            
            ctx.Value.FragColor = value;
        }
    }

    public static void uniform(ShaderType type, out ShaderObject obj)
    {
        int index = ctx.Value.Unifroms.Count;
        obj = new ShaderObject(
            type,
            $"uniform{index}"
        );

        ctx.Value.Unifroms.Add(obj);
    }

    private static ShaderObject func(
        ShaderType returnType,
        string name,
        params (ShaderObject input, ShaderType expectedType)[] inputs
    )
    {
        foreach (var input in inputs)
            validateInput(input.input, input.expectedType, name);
        
        return buildObject(name, returnType, 
            inputs.Select(x => x.input).ToArray()
        );
    }
    
    private static void validateInput(
        ShaderObject input,
        ShaderType expectedType,
        string funcName
    )
    {
        if (input is null)
            throw new Exception($"The input of {funcName} function cannot be null.");

        if ((input.Type & expectedType) == ShaderType.None)
            throw new Exception($"{funcName} function only accepts {expectedType} values.");
    }

    private static ShaderObject buildObject(
        string funcName,
        ShaderType returnType,
        params ShaderObject[] inputs
    )
    {
        var exp = $"{funcName}(";
        for (int i = 0; i < inputs.Length - 1; i++)
            exp += inputs[i].Value + ", ";
        if (inputs.Length > 0)
            exp += inputs[^1].Value + ")";

        return new ShaderObject(
            returnType,
            null,
            exp
        );
    }
}