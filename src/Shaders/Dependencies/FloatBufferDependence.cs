/* Author:  Leonardo Trevisan Silio
 * Date:    28/10/2024
 */
using System;
using System.Text;

namespace Radiance.Shaders.Dependencies;

using Contexts;

/// <summary>
/// Represents a dependence of a float value on buffer layout.
/// </summary>
public class FloatBufferDependence(string name, int location) : ShaderDependence
{
    public readonly string Name = name; 
    public readonly int Location = location; 
    
    public override void AddVertexHeader(StringBuilder sb)
        => sb.AppendLine($"layout (location = {Location}) in float {Name};");

    public override Action AddConfiguration(IShaderConfiguration ctx)
        => () => ctx.AddLayout(1, DataType.Float);

    public override int GetOrderFactor()
        => int.MinValue / 2 + Location;
}