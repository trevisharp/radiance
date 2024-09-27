/* Author:  Leonardo Trevisan Silio
 * Date:    30/08/2024
 */
namespace Radiance.Windows;

public abstract class WindowBuilder
{
    public abstract BaseWindow New(bool fullscreen);
}