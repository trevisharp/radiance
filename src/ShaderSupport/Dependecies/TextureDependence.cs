/* Author:  Leonardo Trevisan Silio
 * Date:    04/01/2023
 */
using System.IO;

using StbImageSharp;

namespace Radiance.ShaderSupport.Dependencies;

using Objects;

/// <summary>
/// Represents a dependece of a Sampler2D used by textures in OpenGL.
/// </summary>
public class TextureDependence : ShaderDependence<Sampler2DShaderObject>
{
    private static int textureCount = -1;
    private static string getTextureId()
    {
        textureCount++;
        return $"texture{textureCount}";
    }

    private static bool initializated = false;
    private static void init()
    {
        if (initializated)
            return;
        
        initializated = true;
        StbImage.stbi_set_flip_vertically_on_load(1);
    }

    ImageResult img;
    public TextureDependence(string imgPath)
    {
        
        if (!File.Exists(imgPath))
            throw new FileNotFoundException();
        
        init();
        this.img = ImageResult.FromStream(
            File.OpenRead(imgPath),
            ColorComponents.RedGreenBlueAlpha
        );
        this.DependenceType = ShaderDependenceType.Uniform;
        this.Name = getTextureId();
    }

    public override object Value => img;

    public override string GetHeader()
        => $"uniform sampler2D {Name};";
}