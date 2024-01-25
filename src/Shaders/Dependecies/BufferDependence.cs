/* Author:  Leonardo Trevisan Silio
 * Date:    25/01/2024
 */
using System.Text;

namespace Radiance.Shaders.Dependencies;

using System;
using Data;

/// <summary>
/// Represents a dependece of a generic buffer data.
/// </summary>
public class BufferDependence : ShaderDependence
{
    Polygon polygon = null;
    public override void AddVertexHeader(StringBuilder sb)
    {
        if (polygon is null)
        {
            sb.AppendLine($"layout (location = 0) in vec3 pos;");
            return;
        }
        
        sb.AppendLine(polygon.Header);
    }

    public override void UpdateData(object value)
        => this.polygon = value as Polygon;
}