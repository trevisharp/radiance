/* Author:  Leonardo Trevisan Silio
 * Date:    15/08/2023
 */
namespace Radiance.ShaderSupport.Dependencies;

using Objects;

/// <summary>
/// Represents a dependece of a position buffer data.
/// </summary>
public class PositionBufferDependence : ShaderDependence<Vec3ShaderObject>
{
    private float[] data;

    public PositionBufferDependence(float[] data)
    {
        this.data = data;

        this.Name = "position";
        this.DependenceType = ShaderDependenceType.CustomData;
    }

    public override object Value
        => this.data;

    public override string GetHeader()
        => $"layout(location = 0) in vec3 position;";
}