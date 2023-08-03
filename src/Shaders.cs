/* Author:  Leonardo Trevisan Silio
 * Date:    03/08/2023
 */
using System.Threading;

namespace DuckGL;

using ShaderSupport;
using ShaderSupport.Types;
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
        => new Vec3ShaderType();
    
    public static ShaderType vec4
        => new Vec4ShaderType();

    public static Vec4ShaderObject vec(ShaderVariable var, float f)
    {
        return new Vec4ShaderObject();
    }

    public static void version(string version)
    {

    }

    public static void layout(int pos, ShaderType type, out ShaderVariable variable)
    {
        variable = new ShaderVariable();
    }

    public static void outs(ShaderType type, out ShaderVariable variable)
    {
        variable = new ShaderVariable();
    }

    public static Vec4ShaderObject gl_Position { get; set; }

    public static void uniform()
    {
        
    }
}