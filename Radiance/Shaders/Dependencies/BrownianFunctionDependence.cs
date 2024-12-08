/* Author:  Leonardo Trevisan Silio
 * Date:    20/09/2024
 */
using System.Collections.Generic;
using System.Text;

namespace Radiance.Shaders.Dependencies;

/// <summary>
/// Represents a dependence of a Fractal Brownian Motion function implementation
/// Source: @patriciogv on https://thebookofshaders.com/13, 2015
/// </summary>
public class BrownianFunctionDependence : ShaderDependence
{
    public override IEnumerable<ShaderDependence> AddDependences()
        => [NoiseDep];

    public override void AddFunctions(StringBuilder sb)
    {
        sb.AppendLine(
            """
            float fbm(in vec2 st) {
                float value = 0.0;
                float amplitude = .5;
                
                for (int i = 0; i < 6; i++) {
                    value += amplitude * noise(st);
                    st *= 2.;
                    amplitude *= .5;
                }
                return value;
            }
            """
        );
    }
}