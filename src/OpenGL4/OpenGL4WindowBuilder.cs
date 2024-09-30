/* Author:  Leonardo Trevisan Silio
 * Date:    30/08/2024
 */
namespace Radiance.OpenGL4;

using Windows;

public class OpenGL4WindowBuilder : WindowBuilder
{
    public override BaseWindow New(bool fullscreen)
        => new OpenGLWindow(fullscreen);
}