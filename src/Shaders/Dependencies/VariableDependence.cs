/* Author:  Leonardo Trevisan Silio
 * Date:    24/01/2024
 */
using System.Text;

namespace Radiance.Shaders.Dependencies;

using CodeGeneration;

/// <summary>
/// Represents dependence of a variable definition in shader code.
/// </summary>
public class VariableDependence(string type, string name, string expr) : ShaderDependence
{
    public string Name => name;
    private readonly string type = type, name = name, expr = expr;

    public VariableDependence(string type, string expr) : this(type, AutoVariableName.Next(type), expr) { }

    public VariableDependence(ShaderObject obj)
        : this(obj.Type.TypeName, obj.Expression) {}

    public override void AddCode(StringBuilder sb)
        => sb.AppendLine($"\t{type} {name} = {expr};");
}