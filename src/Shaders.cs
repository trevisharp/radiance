/* Author:  Leonardo Trevisan Silio
 * Date:    03/08/2023
 */
using System.Threading;

namespace DuckGL;

using ShaderSupport;
using ShaderSupport.Types;

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

    public static void version(string version)
    {
        
    }

    public static void layout(int pos, ShaderType type, out ShaderVariable variable)
    {
        variable = new ShaderVariable();
    }

    public static void uniform()
    {
        
    }
}