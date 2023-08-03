using System.Threading;

namespace DuckGL;

using ShaderSupport;

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

    public static void layout(int pos)
    {

    }

    public static void uniform()
    {
        
    }
}