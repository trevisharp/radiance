/* Author:  Leonardo Trevisan Silio
 * Date:    24/01/2024
 */
using System.Text;

namespace Radiance.Shaders.Dependencies;

using Internal;

/// <summary>
/// Represents a dependence of output data from Vertex Shader to Fragment Shader.
/// </summary>
public class OutputDependence(ShaderObject obj) : ShaderDependence
{
    private readonly string type = obj.Type.TypeName;
    private readonly string name = AutoVariableName.Next(
        "out" + obj.Type.TypeName, 7
    );

    public override void AddVertexCode(StringBuilder sb)
        => sb.AppendLine($"\t{name} = {obj.Expression};");

    public override void AddVertexHeader(StringBuilder sb)
        => sb.AppendLine($"out {type} {name};");

    public override void AddFragmentHeader(StringBuilder sb)
        => sb.AppendLine($"in {type} {name};");
}