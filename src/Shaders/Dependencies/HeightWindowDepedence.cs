/* Author:  Leonardo Trevisan Silio
 * Date:    24/01/2024
 */
using System;
using System.Text;

namespace Radiance.Shaders.Dependencies;

using Contexts;

/// <summary>
/// Represents a dependence of the height of the screen.
/// </summary>
public class HeightWindowDependence : ShaderDependence
{
    public override void AddHeader(StringBuilder sb)
        => sb.AppendLine("uniform float height;");

    public override Action AddOperation(ShaderContext ctx)
        => () => ctx.SetFloat("height", Window.Height);
}