/* Author:  Leonardo Trevisan Silio
 * Date:    27/08/2024
 */
using System;
using System.Text;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using static System.Console;

using OpenTK.Graphics.OpenGL4;
using OpenTKShaderType = OpenTK.Graphics.OpenGL4.ShaderType;

namespace Radiance.Renders;

using Data;
using Shaders;
using Shaders.Objects;

/// <summary>
/// A context used to shader construction..
/// </summary>
public class RenderContext
{
    private event Action<Polygon, object[]> DrawOperations;

    public bool IsVerbose { get; set; } = false;
    public string VersionText { get; set; } = "330 core";
    public Vec3ShaderObject Position { get; set; }
    public Vec4ShaderObject Color { get; set; }
    
    public void Render(Polygon polygon, object[] parameters)
    {
        if (DrawOperations is null)
            return;
        
        DrawOperations(polygon, parameters);
    }

    public void AddClear(Vec4 color)
    {
        DrawOperations += delegate
        {
            GL.ClearColor(
                color.X,
                color.Y,
                color.Z,
                color.W
            );
        };
    }

    public void AddPoints() 
        => AddDrawOperation(PrimitiveType.Points);

    public void AddLines() 
        => AddDrawOperation(PrimitiveType.Lines);
    
    public void AddDraw() 
        => AddDrawOperation(PrimitiveType.LineLoop);
    
    public void AddFill()
        => AddDrawOperation(PrimitiveType.Triangles, true);
    
    public void AddTriangules() 
        => AddDrawOperation(PrimitiveType.Triangles);
    
    public void AddStrip() 
        => AddDrawOperation(PrimitiveType.TriangleStrip);
    
    public void AddFan() 
        => AddDrawOperation(PrimitiveType.TriangleFan);

    private void AddDrawOperation(
        PrimitiveType primitive, 
        bool needTriangularization = false
    )
    {
        var shaderCtx = new ShaderContext();
        var (vertSource, vertSetup, fragSoruce, fragSetup) = 
            GenerateShaders(Position, Color);
        
        var program = RenderProgram.CreateProgram(
            vertSource, fragSoruce, IsVerbose
        );
        shaderCtx.Program = program;
        
        DrawOperations += (poly, data) =>
        {
            if (needTriangularization)
                poly = poly.Triangulation;

            shaderCtx.CreateResources(poly);
            GL.UseProgram(program);

            shaderCtx.Use(poly);

            if (vertSetup is not null)
                vertSetup();

            if (fragSetup is not null)
                fragSetup();

            GL.DrawArrays(primitive, 0, poly.Data.Count() / 3);
        };
    }

    private (string vertSrc, Action vertStp, string fragSrc, Action fragStp) GenerateShaders(
        Vec3ShaderObject vertObj, Vec4ShaderObject fragObj
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
            vertStp += dep.AddOperation(this);
        }

        foreach (var dep in fragDeps)
        {
            dep.AddHeader(fragSb);
            vertStp += dep.AddOperation(this);
        }
        
        foreach (var dep in allDeps)
        {
            dep.AddVertexHeader(vertSb);
            dep.AddFragmentHeader(fragSb);

            vertStp += dep.AddVertexOperation(this);
            fragStp += dep.AddFragmentOperation(this);
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