/* Author:  Leonardo Trevisan Silio
 * Date:    24/10/2024
 */
namespace Radiance.Implementations.OpenGL4;

using Windows;
using Contexts;
using CodeGeneration;
using CodeGeneration.GLSL;

/// <summary>
/// A Builder for a ShaderContext using OpenLG4.
/// </summary>
public class OpenGL4Factory : IImplementationFactory
{
    public ShaderContext NewContext()
        => new OpenGL4ShaderContext();

    public BaseWindow NewWindow()
        => new OpenGL4Window();

    public ICodeGenerator NewCodeGenerator()
        => new GLSLGenerator();
}