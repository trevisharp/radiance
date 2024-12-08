/* Author:  Leonardo Trevisan Silio
 * Date:    25/01/2024
 */
using System;
using System.Text;

namespace Radiance.Shaders.Dependencies;

using Contexts;
using Exceptions;

/// <summary>
/// Represent a parameter in a function used to create a render.
/// </summary>
public class UniformFloatDependence(string name) : ShaderDependence
{
    private float value = 0f;
    public override void AddHeader(StringBuilder sb)
        => sb.AppendLine($"uniform float {name};");

    public override Action AddOperation(IShaderConfiguration ctx)
        => () => ctx.SetFloat(name, value);

    public override void UpdateData(object value)
    {
        if (value is not float num)
            throw new InvalidUniformTypeException(value, "float");
        this.value = num;
    }
}