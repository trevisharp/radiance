/* Author:  Leonardo Trevisan Silio
 * Date:    20/09/2024
 */
using System.Collections.Generic;
using System.Text;

namespace Radiance.Shaders.Dependencies;

/// <summary>
/// Represents a dependence of a noise function implementation
/// Source: @patriciogv on https://thebookofshaders.com/13, 2015
/// </summary>
public class NoiseFunctionDependence : ShaderDependence
{
    public override IEnumerable<ShaderDependence> AddDependences()
        => [RandDep];

    public override void AddFunctions(StringBuilder sb)
    {
        sb.AppendLine(
            """
            float noise (in vec2 st) {
                vec2 i = floor(st);
                vec2 f = fract(st);

                float a = rand(i);
                float b = rand(i + vec2(1.0, 0.0));
                float c = rand(i + vec2(0.0, 1.0));
                float d = rand(i + vec2(1.0, 1.0));

                vec2 u = f * f * (3.0 - 2.0 * f);

                return mix(a, b, u.x) +
                    (c - a)* u.y * (1.0 - u.x) +
                    (d - b) * u.x * u.y;
            }
            """
        );
    }
}