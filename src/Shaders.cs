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
        layout (location = 0) in vec3 pos;

        void main()
        {
            gl_Position = vec4(pos, 1.0);
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

    public static ShaderObject inVar(
        ShaderType type,
        string name
    )
    {
        var obj = new ShaderObject(
            type,
            name
        );

        ctx.Value.InVariables.Add(obj);

        return obj;
    }
    
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
}