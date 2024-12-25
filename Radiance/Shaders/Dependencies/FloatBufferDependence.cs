/* Author:  Leonardo Trevisan Silio
 * Date:    28/10/2024
 */
using System.Text;

namespace Radiance.Shaders.Dependencies;
/// <summary>
/// Represents a dependence of a float value on buffer layout.
/// </summary>
public class FloatBufferDependence(string name, int location) : ShaderDependence
{
    public readonly string Name = name; 
    public readonly int Location = location; 
    
    public override void AddVertexHeader(StringBuilder sb)
        => sb.AppendLine($"layout (location = {Location}) in float {Name};");

    public override int GetOrderFactor()
        => int.MinValue / 2 + Location;
}