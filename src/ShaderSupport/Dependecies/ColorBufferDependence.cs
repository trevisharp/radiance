/* Author:  Leonardo Trevisan Silio
 * Date:    17/08/2023
 */
namespace Radiance.ShaderSupport.Dependencies;

using Objects;

/// <summary>
/// Represents a dependece of a position buffer data.
/// </summary>
public class ColorBufferDependence : ShaderDependence<Vec3ShaderObject>
{
    private float[] data;
    private int position;

    public ColorBufferDependence(float[] data, int position = 1)
    {
        this.data = data;
        this.position = position;

        this.Name = "color";
        this.DependenceType = ShaderDependenceType.CustomData;
    }

    public override object Value
        => this.data;

    public override string GetHeader()
        => $"layout(location = {position}) in vec3 color;";
}