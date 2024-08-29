/* Author:  Leonardo Trevisan Silio
 * Date:    26/02/2024
 */
using System.Text;

namespace Radiance.Shaders.Dependencies;

/// <summary>
/// Represents a dependece of a generic buffer data.
/// </summary>
public class BufferDependence : ShaderDependence
{
    public override void AddVertexHeader(StringBuilder sb)
    {
        sb.AppendLine($"layout (location = 0) in vec3 pos;");
        return;
    }
}