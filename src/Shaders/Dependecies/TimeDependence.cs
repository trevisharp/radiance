/* Author:  Leonardo Trevisan Silio
 * Date:    24/01/2024
 */
using System;
using System.Text;

namespace Radiance.Shaders.Dependencies;

/// <summary>
/// Represents a dependence of the time of the application.
/// </summary>
public class TimeDependence : ShaderDependence
{
    DateTime start = DateTime.Now;
    public DateTime ZeroTime => start;
    public float Seconds => (float)(DateTime.Now - start).TotalSeconds;
        
    public override void AddHeader(StringBuilder sb)
        => sb.AppendLine("uniform float t;");

    public override Action AddOperation(ShaderContext ctx)
        => () => ctx.SetFloat("t", Seconds);
}