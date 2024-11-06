/* Author:  Leonardo Trevisan Silio
 * Date:    06/11/2024
 */
namespace Radiance.Implementations;

using Windows;
using Contexts;

/// <summary>
/// A interface for all implementation factories.
/// </summary>
public interface IImplementationFactory
{
    /// <summary>
    /// Build the shader context engine to manager shader programs.
    /// </summary>
    ShaderContext Build();

    /// <summary>
    /// Create a window.
    /// </summary>
    BaseWindow New();
}