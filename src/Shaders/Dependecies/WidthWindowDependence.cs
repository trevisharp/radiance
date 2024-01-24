/* Author:  Leonardo Trevisan Silio
 * Date:    24/01/2024
 */
using System;
using System.Text;

namespace Radiance.Shaders.Dependencies;

/// <summary>
/// Represents a dependence of the width of the screen.
/// </summary>
public class WidthWindowDependence : ShaderDependence
{
    public override void AddHeader(StringBuilder sb)
        => sb.AppendLine("uniform float width;");

    public override Action AddOperation(ShaderContext ctx)
        => () => ctx.SetUniformFloat("width", (float)Window.Width);
}