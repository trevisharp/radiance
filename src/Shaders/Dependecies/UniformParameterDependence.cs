/* Author:  Leonardo Trevisan Silio
 * Date:    25/01/2024
 */
using System.Text;

namespace Radiance.Shaders.Dependencies;

/// <summary>
/// Represent a parameter in a function used to create a render.
/// </summary>
public class UniformParameterDependence(string name, string type) : ShaderDependence
{
    public override void AddHeader(StringBuilder sb)
        => sb.AppendLine($"uniform {type} {name};");
}