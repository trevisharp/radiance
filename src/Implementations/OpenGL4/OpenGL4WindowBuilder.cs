/* Author:  Leonardo Trevisan Silio
 * Date:    30/08/2024
 */
namespace Radiance.Implementations.OpenGL4;

using Windows;

/// <summary>
/// A Builder for a window implementation using OpenLG4.
/// </summary>
public class OpenGL4WindowBuilder : WindowBuilder
{
    public override BaseWindow New(bool fullscreen)
        => new OpenGL4Window(fullscreen);
}