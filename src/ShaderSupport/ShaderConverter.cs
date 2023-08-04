/* Author:  Leonardo Trevisan Silio
 * Date:    03/08/2023
 */
using System.Text;

namespace DuckGL.ShaderSupport;

public static class ShaderConverter
{
    public static string ToShader(ShaderContext ctx)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("#version " + ctx.Version);
        foreach (var layout in ctx.Layout)
        {
            int p = layout.pos;
            string type = typeToString(layout.type);
            sb.AppendLine(
                $"layout (location = {p}) in {type} data{p}"
            );
        }

        return sb.ToString();
    }

    private static string typeToString(ShaderType type)
        => type switch
        {
            ShaderType.Float => "float",
            ShaderType.Vec2 => "vec2",
            ShaderType.Vec3 => "vec3",
            ShaderType.Vec4 => "vec4",
            _ => "unknow"
        };
}