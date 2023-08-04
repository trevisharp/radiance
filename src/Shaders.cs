/* Author:  Leonardo Trevisan Silio
 * Date:    03/08/2023
 */
using System;
using System.Threading;

namespace DuckGL;

using ShaderSupport;
using ShaderSupport.Objects;

public static class Shaders
{
    static ThreadLocal<ShaderContext> ctx;
    static Shaders()
    {
        ctx = new ThreadLocal<ShaderContext>(
            () => new ShaderContext()
        );
    }

    internal static void ResetContext()
        => ctx.Value = new ShaderContext();

    internal static ShaderContext GetContext()
        => ctx.Value;
    
    public static ShaderType vec3
        => ShaderType.Vec3;
    
    public static ShaderType vec4
        => ShaderType.Vec4;

    public static ShaderObject vec(
        ShaderObject var,
        float f
    )
    {
        var data = var.Name ?? var.Expression;

        switch (var.Type)
        {
            case ShaderType.Vec2:
                return new Vec3ShaderObject(
                    null,
                    $"vec3({data}, {f})"
                );
            
            case ShaderType.Vec3:
                return new Vec4ShaderObject(
                    null,
                    $"vec4({data}, {f})"
                );
            
            default:
                throw new Exception($"'{data}' is invalid to use in vec");
        }
    }

    public static void version(string version)
    {

    }

    public static void layout(
        int pos,
        ShaderType type,
        out ShaderObject variable)
    {
        variable = new ShaderObject(
            type
        );
    }

    public static void outVar(
        ShaderType type,
        string name, 
        ShaderObject obj
    )
    {

    }

    public static ShaderObject inVar(
        ShaderType type,
        string name
    )
    {
        var obj = new ShaderObject(
            type
        );

        return obj;
    }
    public static Vec4ShaderObject gl_Position { get; set; }
    public static Vec4ShaderObject gl_FragColor { get; set; }

    public static void uniform(ShaderType type, out ShaderObject obj)
    {
        obj = new ShaderObject(
            type
        );
    }
}