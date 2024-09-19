/* Author:  Leonardo Trevisan Silio
 * Date:    19/09/2024
 */
using System;

namespace Radiance.Shaders.Dependencies;

using Managers;

/// <summary>
/// Represents a Constant dependence.
/// </summary>
public class ConstantDependence(string name, float value) : ShaderDependence
{
    bool setted = false;

    public override Action AddOperation(ShaderManager ctx)
        => () => {
            if (setted)
                return;
            
            setted = true;
            ctx.SetFloat(name, value);
        };
}