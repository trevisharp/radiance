/* Author:  Leonardo Trevisan Silio
 * Date:    09/01/2023
 */
using System.IO;

using StbImageSharp;

namespace Radiance.Shaders.Dependencies;

using Objects;

/// <summary>
/// Represents a dependece of a Sampler2D used by textures in OpenGL.
/// </summary>
public class TextureDependence : ShaderDependence<Sampler2DShaderObject>
{
    private static int textureCount = -1;
    private static int getTextureId()
    {
        textureCount++;
        return textureCount;
    }

    private static bool initializated = false;
    private static void init()
    {
        if (initializated)
            return;
        
        initializated = true;
        StbImage.stbi_set_flip_vertically_on_load(1);
    }

    public ImageResult Image { get; set; }
    public TextureDependence(string imgPath)
    {
        if (!File.Exists(imgPath))
            throw new FileNotFoundException();
        
        init();
        this.Image = ImageResult.FromStream(
            File.OpenRead(imgPath),
            ColorComponents.RedGreenBlueAlpha
        );
        this.DependenceType = ShaderDependenceType.Texture;
        this.Name = $"texture{getTextureId()}";
    }

    public override object Value => Image;

    public override string GetHeader()
        => $"uniform sampler2D {this.Name};";
}