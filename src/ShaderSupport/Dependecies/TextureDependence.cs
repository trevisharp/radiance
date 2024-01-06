/* Author:  Leonardo Trevisan Silio
 * Date:    04/01/2023
 */
namespace Radiance.ShaderSupport.Objects;

/// <summary>
/// Represents a dependece of a Sampler2D used by textures in OpenGL.
/// </summary>
public class TextureDependence : ShaderDependence
{
    private string textureID;
    public TextureDependence(string textureID)
        => this.textureID = textureID;

    public override object Value => throw new System.NotImplementedException();

    public override string GetHeader()
        => $"uniform sampler2D {textureID};";
}