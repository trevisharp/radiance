/* Author:  Leonardo Trevisan Silio
 * Date:    07/12/2024
 */
using System.Text;
using System.Collections.Generic;

namespace Radiance.Shaders.Dependencies;

using CodeGeneration;

/// <summary>
/// Represents a dependence of output data from Vertex Shader to Fragment Shader.
/// </summary>
public class OutputDependence(ShaderObject obj) : ShaderDependence
{
    private readonly bool isBool = obj.Type == ShaderType.Bool;
    private readonly string type = obj.Type == ShaderType.Bool ? ShaderType.Float.TypeName : obj.Type.TypeName;
    public readonly string Name = AutoVariableName.Next(
        "out" + obj.Type.TypeName, 7
    );

    /// <summary>
    /// Get the expression that reads the output on the fragment shader.
    /// Booleans travel as float, since GLSL does not allow bool outputs.
    /// </summary>
    public string Access => isBool ? $"({Name} > 0.5)" : Name;

    public override void AddVertexCode(StringBuilder sb)
        => sb.AppendLine(isBool ? $"\t{Name} = float({obj.Expression});" : $"\t{Name} = {obj.Expression};");

    public override void AddVertexHeader(StringBuilder sb)
        => sb.AppendLine($"out {type} {Name};");

    public override void AddFragmentHeader(StringBuilder sb)
        => sb.AppendLine($"in {type} {Name};");

    public override IEnumerable<ShaderDependence> AddVertexDependences()
        => obj.Dependencies;
}