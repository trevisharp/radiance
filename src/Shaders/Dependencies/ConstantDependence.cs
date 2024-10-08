/* Author:  Leonardo Trevisan Silio
 * Date:    19/09/2024
 */
using System;
using System.Text;

namespace Radiance.Shaders.Dependencies;

using Contexts;

/// <summary>
/// Represents a Constant dependence.
/// </summary>
public class ConstantDependence(string name, float value) : ShaderDependence
{
    bool setted = false;
    public override void AddHeader(StringBuilder sb)
        => sb.AppendLine($"uniform float {name};");

    public override Action AddOperation(ShaderContext ctx)
        => () => {
            if (setted)
                return;
            
            setted = true;
            ctx.SetFloat(name, value);
        };
}