/* Author:  Leonardo Trevisan Silio
 * Date:    24/10/2024
 */
namespace Radiance.Implementations.OpenGL4;

using Contexts;
using Radiance.Windows;

/// <summary>
/// A Builder for a ShaderContext using OpenLG4.
/// </summary>
public class OpenGL4Factory : IImplementationFactory
{
    public ShaderContext Build()
        => new OpenGL4ShaderContext();

    public BaseWindow New()
        => new OpenGL4Window();
}