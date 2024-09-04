/* Author:  Leonardo Trevisan Silio
 * Date:    29/08/2023
 */
using System.IO;

using StbImageSharp;

namespace Radiance.Primitives;

/// <summary>
/// Represents a texture used by shaders.
/// </summary>
public class Texture(string imgPath)
{
    public readonly string Source = imgPath;
    public readonly ImageResult ImageData = Load(imgPath);

    static bool initializated = false;
    static void Init()
    {
        if (initializated)
            return;
        
        initializated = true;
        StbImage.stbi_set_flip_vertically_on_load(1);
    }

    static ImageResult Load(string imgPath)
    {
        if (!File.Exists(imgPath))
            throw new FileNotFoundException();

        Init();
        return ImageResult.FromStream(
            File.OpenRead(imgPath),
            ColorComponents.RedGreenBlueAlpha
        );
    }
}