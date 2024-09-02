/* Author:  Leonardo Trevisan Silio
 * Date:    24/01/2024
 */
using System.Text;

namespace Radiance.Shaders.Dependencies;

using CodeGen;

/// <summary>
/// Represents dependence of a variable definition in shader code.
/// </summary>
public class VariableDependence : ShaderDependence
{
    public string Name => this.name;

    string type, name, expr;
    public VariableDependence(string type, string name, string expr)
    {
        this.type = type;
        this.name = name;
        this.expr = expr;
    }

    public VariableDependence(string type, string expr)
    {
        this.type = type;
        this.name = AutoVariableName.Next(type);
        this.expr = expr;
    }

    public VariableDependence(ShaderObject obj)
        : this(obj.Type.TypeName, obj.Expression) {}

    public override void AddCode(StringBuilder sb)
        => sb.AppendLine($"\t{type} {name} = {expr};");
}