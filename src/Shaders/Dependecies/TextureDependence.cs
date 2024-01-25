/* Author:  Leonardo Trevisan Silio
 * Date:    25/01/2023
 */
namespace Radiance.Shaders.Dependencies;

using Data;
using Objects;

/// <summary>
/// Represents a dependece of a Sampler2D used by textures in OpenGL.
/// </summary>
public class TextureDependence : ShaderDependence
{
    private static int textureCount = -1;
    private static int getTextureId()
    {
        textureCount++;
        return textureCount;
    }

    private Texture texture;
    public TextureDependence()
    {
        this.DependenceType = ShaderDependenceType.Texture;
        this.Name = $"texture{getTextureId()}";
    }

    public override object Value => texture;

    public override string GetHeader()
        => $"uniform sampler2D {this.Name};";

    public override void UpdateValue(object newValue)
        => texture = newValue as Texture;
}