/* Author:  Leonardo Trevisan Silio
 * Date:    06/11/2024
 */
namespace Radiance.Implementations.Vulkan13;

using Contexts;
using Windows;

/// <summary>
/// A Builder for a ShaderContext using Vulkan 1.3.
/// </summary>
public class Vulkan13Factory : IImplementationFactory
{
    public ShaderContext Build()
    {
        throw new System.NotImplementedException(
            "The implementation of Vulkan 1.3 not exists on this version of Radiance."
        );
    }

    public BaseWindow New()
    {
        throw new System.NotImplementedException(
            "The implementation of Vulkan 1.3 not exists on this version of Radiance."
        );
    }
}