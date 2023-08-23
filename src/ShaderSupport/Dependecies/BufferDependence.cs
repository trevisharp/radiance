/* Author:  Leonardo Trevisan Silio
 * Date:    23/08/2023
 */
namespace Radiance.ShaderSupport.Dependencies;

using Data;
using Objects;

/// <summary>
/// Represents a dependece of a generic buffer data.
/// </summary>
public class BufferDependence<T> : ShaderDependence<T>
    where T : ShaderObject, new()
{
    private IData data;
    private float[] buffer;
    private int position;
    private string header;

    public BufferDependence(string name, IData data, int position = 1)
    {
        this.data = data;
        this.position = position;
        this.buffer = data.GetBuffer();

        this.Name = name;
        this.DependenceType = ShaderDependenceType.CustomData;

        this.header = $"layout(location = {position}) in {ShaderObject.GetStringName<T>()} {Name};";
    }

    public override object Value
        => this.buffer;

    public override string GetHeader()
        => this.header;
}