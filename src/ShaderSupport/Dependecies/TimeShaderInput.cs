/* Author:  Leonardo Trevisan Silio
 * Date:    10/08/2023
 */
using System;

namespace Radiance.ShaderSupport.Dependencies;

/// <summary>
/// Represents a input for shaders of the time 
/// passed since the creation of this input in seconds.
/// </summary>
public class TimeShaderInput : ShaderInput
{
    DateTime start;

    public TimeShaderInput()
    {
        this.start = DateTime.Now;
        this.Name = "t";
        this.Type = ShaderType.Float;
        this.DependenceType = ShaderDependenceType.Uniform;
    }

    public override object Value
        => (float)(DateTime.Now - start).TotalSeconds;
}