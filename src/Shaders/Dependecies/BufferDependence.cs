/* Author:  Leonardo Trevisan Silio
 * Date:    19/01/2024
 */
namespace Radiance.Shaders.Dependencies;

using Data;

/// <summary>
/// Represents a dependece of a generic buffer data.
/// </summary>
public class BufferDependence<T> : OldShaderDependence<T>
    where T : ShaderObject, new()
{
    private float[] buffer;
    private string header;
    
    public BufferDependence(string name, Polygon poly, int location = 0)
    {
        this.Name = name;
        this.DependenceType = ShaderDependenceType.CustomData;

        this.header = $"layout(location = {location}) in {ShaderObject.GetStringName<T>()} {Name};";
    }

    public override object Value
        => this.buffer;

    public override string GetHeader()
        => this.header;
}