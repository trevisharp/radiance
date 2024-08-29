/* Author:  Leonardo Trevisan Silio
 * Date:    29/01/2024
 */
using System.Text;

namespace Radiance.Shaders.Dependencies;

/// <summary>
/// Represents a dependence of pixel position.
/// </summary>
public class PixelDependence : ShaderDependence
{
    public override void AddVertexFinalCode(StringBuilder sb)
        => sb.AppendLine($"\tpixelPos = finalPosition;");

    public override void AddVertexHeader(StringBuilder sb)
        => sb.AppendLine($"out vec3 pixelPos;");

    public override void AddFragmentHeader(StringBuilder sb)
        => sb.AppendLine($"in vec3 pixelPos;");
}