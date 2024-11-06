/* Author:  Leonardo Trevisan Silio
 * Date:    06/11/2024
 */
namespace Radiance.Implementations.Vulkan13;

using Windows;

/// <summary>
/// A Builder for a window implementation using Vulkan 1.3.
/// </summary>
public class Vulkan13WindowBuilder : WindowBuilder
{
    public override BaseWindow New(bool fullscreen)
    {
        throw new System.NotImplementedException(
            "The implementation of Vulkan 1.3 not exists on this version of Radiance."
        );
    }
}