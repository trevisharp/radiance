/* Author:  Leonardo Trevisan Silio
 * Date:    04/08/2023
 */
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace DuckGL.ShaderSupport;

/// <summary>
/// A Helper to convert a ShaderContext a string GLSL code.
/// </summary>
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
                $"layout (location = {p}) in {type} data{p};"
            );
        }

        if (ctx.FragColor is not null)
        {
            sb.AppendLine(
                "out vec4 FragColor;"
            );
        }

        generateMembers("uniform", ctx.Unifroms, sb);
        generateMembers("in", ctx.InVariables, sb);
        generateMembers("out", ctx.OutVariables.Select(x => x.Item1), sb);

        sb.AppendLine();
        sb.AppendLine("void main()");
        sb.AppendLine("{");

        foreach (var outVar in ctx.OutVariables)
        {
            var name = outVar.Item1.Value;
            var value = outVar.Item2.Value;

            sb.AppendLine(
                $"\t{name} = {value};"
            );
        }

        if (ctx.Position is not null)
        {
            sb.AppendLine(
                $"\tgl_Position = {ctx.Position.Value};"
            );
        }

        if (ctx.FragColor is not null)
        {
            sb.AppendLine(
                $"\tFragColor = {ctx.FragColor.Value};"
            );
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static void generateMembers(
        string memberName,
        IEnumerable<ShaderObject> objs,
        StringBuilder sb
    )
    {
        if (objs.Count() == 0)
            return;
        sb.AppendLine();

        foreach (var obj in objs)
        {
            var value = obj.Value;
            var type = typeToString(obj.Type);

            sb.AppendLine(
                $"{memberName} {type} {value};"
            );
        }
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