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
    public override void AddVertexCode(StringBuilder sb)
    {
        if (polygon is null)
            return;
        
        sb.AppendLine(polygon.Header);
    }

    public override Action AddVertexOperation(ShaderContext ctx)
        => () => Console.Write("TODO");

    public override void UpdateData(object value)
        => this.polygon = value as Polygon;
}