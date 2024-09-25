/* Author:  Leonardo Trevisan Silio
 * Date:    25/09/2024
 */
using System;
using System.Text;

namespace Radiance.Shaders.Dependencies;

using Radiance.Managers;

/// <summary>
/// Represents a dependece of a generic buffer data.
/// </summary>
public class BufferDependence : ShaderDependence
{
    public override void AddVertexHeader(StringBuilder sb)
        => sb.AppendLine($"layout (location = 0) in vec3 pos;");

    public override Action AddConfiguration(ShaderManager ctx)
        => () => ctx.AddLayout(3);
}