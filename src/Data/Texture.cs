/* Author:  Leonardo Trevisan Silio
 * Date:    22/01/2023
 */
using System.IO;

using StbImageSharp;

namespace Radiance.Data;

/// <summary>
/// Represents a texture used by shaders.
/// </summary>
public class Texture
{
    private static bool initializated = false;
    private static void init()
    {
        if (initializated)
            return;
        
        initializated = true;
        StbImage.stbi_set_flip_vertically_on_load(1);
    }

    ImageResult img;
    public readonly string Source;
    public Texture(string imgPath)
    {
        if (!File.Exists(imgPath))
            throw new FileNotFoundException();
        
        init();
        this.Source = imgPath;
        this.img = ImageResult.FromStream(
            File.OpenRead(imgPath),
            ColorComponents.RedGreenBlueAlpha
        );
    }

    public ImageResult ImageData => img;
}