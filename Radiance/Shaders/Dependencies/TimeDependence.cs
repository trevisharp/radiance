/* Author:  Leonardo Trevisan Silio
 * Date:    27/08/2024
 */
using System;
using System.Text;

namespace Radiance.Shaders.Dependencies;

using Contexts;

/// <summary>
/// Represents a dependence of the time of the application based
/// on Clock.Shared reference.
/// </summary>
public class TimeDependence : ShaderDependence
{       
    public override void AddHeader(StringBuilder sb)
        => sb.AppendLine("uniform float t;");

    public override Action AddOperation(IShaderConfiguration ctx)
        => () => ctx.SetFloat("t", Clock.Shared.Time);
}