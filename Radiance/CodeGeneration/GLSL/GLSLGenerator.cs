/* Author:  Leonardo Trevisan Silio
 * Date:    04/11/2024
 */
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Radiance.CodeGeneration.GLSL;

using Shaders;
using Contexts;

/// <summary>
/// Tools to generate GL Shader Language Code.
/// </summary>
public class GLSLGenerator : ICodeGenerator
{
    /// <summary>
    /// Get or Set the GSLS Version.
    /// </summary>
    public string VersionText { get; set; } = "330 core";
    
    /// <summary>
    /// Generate a pair of vertex and fragment GLSL Shaders based on Position Shader Object,
    /// a Color Shader Object and a ShaderContext.
    /// </summary>
    public ShaderPair GenerateShaders(
        vec3 vertObj,
        vec4 fragObj,
        IShaderConfiguration ctx,
        GeneratorOptions? options = null)
    {
        options ??= GeneratorOptions.Default;

        StringBuilder getCodeBuilder()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"#version {VersionText}");
            return sb;
        }
        var vertSb = getCodeBuilder();
        var fragSb = getCodeBuilder();

        Action? vertStp = null;
        Action? fragStp = null;
        Action? config = null;

        var vertDeps = vertObj.Dependencies
            .Append(ShaderDependence.WidthDep)
            .Append(ShaderDependence.HeightDep)
            .ToList();
        var fragDeps = fragObj.Dependencies
            .ToList();
        var allDeps = vertDeps.Concat(fragDeps);

        vertDeps.AddRange(
            allDeps.SelectMany(dep => dep.AddVertexDependences())
        );
        fragDeps.AddRange(
            allDeps.SelectMany(dep => dep.AddFragmentDependences())
        );

        vertDeps = [ 
            ..ExpandDeps(vertDeps)
            .Distinct()
            .OrderBy(dep => dep.GetOrderFactor())
        ];
        
        fragDeps = [
            ..ExpandDeps(fragDeps)
            .Distinct()
            .OrderBy(dep => dep.GetOrderFactor())
        ];

        allDeps = vertDeps
            .Concat(fragDeps)
            .OrderBy(dep => dep.GetOrderFactor())
            .Distinct();
        
        foreach (var dep in allDeps)
            config += dep.AddConfiguration(ctx);

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

        foreach (var dep in vertDeps)
            dep.AddFunctions(vertSb);

        foreach (var dep in fragDeps)
            dep.AddFunctions(fragSb);

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
        vertSb.AppendLine($"\tvec3 transformPosition = finalPosition;");

        if (options.PixelBased)
            vertSb.AppendLine($"\ttransformPosition = vec3(2 * transformPosition.x / width - 1, 2 * transformPosition.y / height - 1, transformPosition.z);");

        if (options.LargeZIndex)
            vertSb.AppendLine($"\ttransformPosition = vec3(transformPosition.x, transformPosition.y, 0.99999f - 2 * transformPosition.z / 1001);");

        vertSb.AppendLine($"\tgl_Position = vec4(transformPosition, 1.0);");
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

        var vertexCode = vertSb.ToString();
        var vertexShader = new ShaderCode(vertexCode, vertexCode.GetHashCode(), vertStp);

        var fragmentCode = fragSb.ToString();
        var fragmentShader = new ShaderCode(fragmentCode, fragmentCode.GetHashCode(), fragStp);

        return new(vertexShader, fragmentShader, config);
    }

    static List<ShaderDependence> ExpandDeps(List<ShaderDependence> dependences)
    {
        // TODO: Avaliate dependency cycles.
        var stack = new Stack<ShaderDependence>();
        foreach (var dep in dependences)
            stack.Push(dep);
        
        while (stack.Count > 0)
        {
            var dep = stack.Pop();
            dependences.Insert(0, dep);

            var newDeps = dep.AddDependences();
            foreach (var newDep in newDeps)
                stack.Push(newDep);
        }

        return dependences;
    }
}