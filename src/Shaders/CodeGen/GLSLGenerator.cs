/* Author:  Leonardo Trevisan Silio
 * Date:    02/09/2024
 */
using System;
using System.Text;
using System.Linq;

namespace Radiance.Shaders.CodeGen;

using Objects;
using Dependencies;

/// <summary>
/// Tools to generate GL Shader Language Code.
/// </summary>
public class GLSLGenerator
{
    public string VersionText { get; set; } = "330 core";
        private (string vertSrc, Action vertStp, string fragSrc, Action fragStp) GenerateShaders(
            Vec3ShaderObject vertObj, Vec4ShaderObject fragObj, ShaderContext ctx
        )
    {
        StringBuilder getCodeBuilder()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"#version {VersionText}");
            return sb;
        }
        var vertSb = getCodeBuilder();
        var fragSb = getCodeBuilder();

        Action vertStp = null;
        Action fragStp = null;

        var vertDeps = vertObj.Dependencies
            .Append(Utils.widthDep)
            .Append(Utils.heightDep)
            .Distinct();
        var fragDeps = fragObj.Dependencies
            .Distinct();

        var allDeps = vertDeps
            .Concat(fragDeps)
            .Distinct();

        foreach (var dep in vertDeps)
        {
            dep.AddHeader(vertSb);
            vertStp += dep.AddOperation(ctx);
        }

        foreach (var dep in fragDeps)
        {
            dep.AddHeader(fragSb);
            vertStp += dep.AddOperation(ctx);
        }
        
        foreach (var dep in allDeps)
        {
            dep.AddVertexHeader(vertSb);
            dep.AddFragmentHeader(fragSb);

            vertStp += dep.AddVertexOperation(ctx);
            fragStp += dep.AddFragmentOperation(ctx);
        }
        
        fragSb.AppendLine("out vec4 outColor;");

        void initMain(StringBuilder sb)
        {
            sb.AppendLine();
            sb.AppendLine("void main()");
            sb.AppendLine("{");
        }
        initMain(vertSb);
        initMain(fragSb);

        foreach (var dep in vertDeps)
            dep.AddCode(vertSb);

        foreach (var dep in fragDeps)
            dep.AddCode(fragSb);
        
        foreach (var dep in allDeps)
        {
            dep.AddVertexCode(vertSb);
            dep.AddFragmentCode(fragSb);
        }

        vertSb.AppendLine($"\tvec3 finalPosition = {vertObj};");
        vertSb.AppendLine($"\tvec3 tposition = vec3(2 * finalPosition.x / width - 1, 2 * finalPosition.y / height - 1, finalPosition.z);");
        vertSb.AppendLine($"\tgl_Position = vec4(tposition, 1.0);");
        fragSb.AppendLine($"\toutColor = {fragObj};");

        foreach (var dep in allDeps)
        {
            dep.AddVertexFinalCode(vertSb);
            dep.AddFragmentFinalCode(fragSb);
        }

        foreach (var dep in vertDeps)
            dep.AddFinalCode(vertSb);
        
        foreach (var dep in fragDeps)
            dep.AddFinalCode(fragSb);

        void closeMain(StringBuilder sb)
            => sb.Append('}');
        closeMain(vertSb);
        closeMain(fragSb);

        return (vertSb.ToString(), vertStp, fragSb.ToString(), fragStp);
    }

}