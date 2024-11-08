/* Author:  Leonardo Trevisan Silio
 * Date:    06/11/2024
 */
namespace Radiance.Implementations.Vulkan13;

using Contexts;
using Radiance.CodeGeneration;
using Windows;

/// <summary>
/// A Builder for a ShaderContext using Vulkan 1.3.
/// </summary>
public class Vulkan13Factory : IImplementationFactory
{
    public ShaderContext NewContext()
    {
        throw new System.NotImplementedException(
            "Vulkan 1.3 implementation is missing in this version of Radiance."
        );
    }

    public BaseWindow NewWindow()
    {
        throw new System.NotImplementedException(
            "Vulkan 1.3 implementation is missing in this version of Radiance."
        );
    }

    public ICodeGenerator NewCodeGenerator()
    {
        throw new System.NotImplementedException(
            "Vulkan 1.3 implementation is missing in this version of Radiance."
        );
    }
}