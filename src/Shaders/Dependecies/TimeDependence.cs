/* Author:  Leonardo Trevisan Silio
 * Date:    15/08/2023
 */
using System;

namespace Radiance.Shaders.Dependencies;

using Objects;

/// <summary>
/// Represents a input for shaders of the time 
/// passed since the creation of this input in seconds.
/// </summary>
public class TimeShaderInput : OldShaderDependence<FloatShaderObject>
{
    DateTime start;

    public DateTime ZeroTime => start;

    public TimeShaderInput()
    {
        this.start = DateTime.Now;
        this.Name = "t";
        this.DependenceType = ShaderDependenceType.Uniform;
    }

    public override object Value
        => (float)(DateTime.Now - start).TotalSeconds;

    public override string GetHeader()
        => "uniform float t;";
}