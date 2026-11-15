/* Author:  Leonardo Trevisan Silio
 * Date:    16/11/2026
 */
using System;
using System.Text;

namespace Radiance.Shaders.Dependencies;

using Contexts;

/// <summary>
/// Represents the dependence of a vec2, vec3 or vec4 uniform received from render arguments.
/// </summary>
public class UniformVectorDependence(string name, int size) : ShaderDependence, IFloatArgumentDependence
{
    private readonly float[] values = new float[size];

    public override void AddHeader(StringBuilder sb)
        => sb.AppendLine($"uniform vec{size} {name};");

    public override Action AddOperation(IShaderConfiguration ctx) => size switch
    {
        2 => () => ctx.SetVec(name, values[0], values[1]),
        3 => () => ctx.SetVec(name, values[0], values[1], values[2]),
        _ => () => ctx.SetVec(name, values[0], values[1], values[2], values[3])
    };

    public void SetArgument(int component, float value)
        => values[component] = value;
}
