/* Author:  Leonardo Trevisan Silio
 * Date:    19/09/2024
 */
using System.Text;

namespace Radiance.Shaders.Dependencies;

/// <summary>
/// Represents a dependece of a rand implementation based on fract code.
/// Source: @patriciogv on https://thebookofshaders.com/13, 2015
/// </summary>
public class RandFunctionDependence : ShaderDependence
{
    public override void AddFunctions(StringBuilder sb)
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