/* Author:  Leonardo Trevisan Silio
 * Date:    19/09/2024
 */
using System.Text;

namespace Radiance.Shaders.Dependencies;

/// <summary>
/// Represents a dependece of a rand implementation based on fract code.
/// </summary>
public class RandDependence : ShaderDependence
{
    public override void AddHeader(StringBuilder sb)
    {
        sb.AppendLine(
            """
            float rand(vec2 v) { 
                return fract(sin(dot(v, vec2(12.9898, 4.1414))) * 43758.5453);
            }
            """
        );
    }
}