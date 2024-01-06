/* Author:  Leonardo Trevisan Silio
 * Date:    06/01/2023
 */
using System.IO;

using StbImageSharp;

namespace Radiance.ShaderSupport.Objects;

/// <summary>
/// Represents a Sampler2D used by textures in OpenGL.
/// </summary>
public class Sampler2DShaderObject : ShaderObject
{
    private static int textureCount = 0;
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
    string textureID;
    public Sampler2DShaderObject(string imgPath)
    {
        if (!File.Exists(imgPath))
            throw new FileNotFoundException();
        
        init();
        this.img = ImageResult.FromStream(
            File.OpenRead(imgPath),
            ColorComponents.RedGreenBlueAlpha
        );
        this.textureID = getTextureId();
        this.Dependecies = new ShaderDependence[] {
            new TextureDependence(this.textureID)
        };
    }
}