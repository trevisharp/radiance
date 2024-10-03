/* Author:  Leonardo Trevisan Silio
 * Date:    03/10/2024
 */
using System;
using System.Text;

namespace Radiance.Shaders.Dependencies;

using Contexts;

/// <summary>
/// Represents a dependence of a float value on buffer layout.
/// </summary>
public class FloatBufferDependence(string name, int location) : ShaderDependence
{
    public override void AddVertexHeader(StringBuilder sb)
        => sb.AppendLine($"layout (location = {location}) in float {name};");

    public override Action AddConfiguration(ShaderContext ctx)
        => () => ctx.AddLayout(1);
}