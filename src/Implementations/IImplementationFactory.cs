/* Author:  Leonardo Trevisan Silio
 * Date:    06/11/2024
 */
namespace Radiance.Implementations;

using Windows;
using Contexts;
using CodeGeneration;

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

    /// <summary>
    /// Create a code generator for generate shaders code.
    /// </summary>
    ICodeGenerator NewCodeGenerator();
}