/* Author:  Leonardo Trevisan Silio
 * Date:    24/11/2024
 */
namespace Radiance;

using Primitives;

public static class Textures
{
    /// <summary>
    /// Open a image file to use in your shader.
    /// </summary>
    public static Texture Open(string imgFile)
    {
        var texture = new Texture(imgFile);
        return texture;
    }
}