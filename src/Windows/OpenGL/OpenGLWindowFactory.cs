/* Author:  Leonardo Trevisan Silio
 * Date:    30/08/2024
 */
namespace Radiance.Windows.OpenGL;

public class OpenGLWindowFactory : WindowFactory
{
    public override BaseWindow New(bool fullscreen)
        => new OpenGLWindow(fullscreen);
}