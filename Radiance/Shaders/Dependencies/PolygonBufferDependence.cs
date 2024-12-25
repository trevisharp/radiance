/* Author:  Leonardo Trevisan Silio
 * Date:    25/09/2024
 */
using System.Text;

namespace Radiance.Shaders.Dependencies;
/// <summary>
/// Represents a dependece of a generic buffer data.
/// </summary>
public class PolygonBufferDependence : ShaderDependence
{
    public override void AddVertexHeader(StringBuilder sb)
        => sb.AppendLine($"layout (location = 0) in vec3 pos;");

    public override int GetOrderFactor()
        => int.MinValue / 2;
}