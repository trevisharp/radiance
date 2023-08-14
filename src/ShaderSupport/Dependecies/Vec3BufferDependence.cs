/* Author:  Leonardo Trevisan Silio
 * Date:    14/08/2023
 */
namespace Radiance.ShaderSupport.Dependencies;

/// <summary>
/// Represents a dependece of a buffer data
/// </summary>
public class Vec3BufferDependence : ShaderDependence
{
    private float[] data;

    public Vec3BufferDependence(float[] data)
    {
        this.data = data;

        this.Name = "position";
        this.Type = ShaderType.Vec3;
        this.DependenceType = ShaderDependenceType.Uniform;
    }

    public override object Value
        => this.data;

    public override string GetHeader(params object[] args)
        => $"layout(location = 0) in vec3 position;";
}