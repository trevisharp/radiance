/* Author:  Leonardo Trevisan Silio
 * Date:    12/09/2024
 */
namespace Radiance.Shaders.CodeGeneration.GLSL;

/// <summary>
/// A builder for a GLSL code generator.
/// </summary>
public class GLSLGeneratorBuilder : ICodeGeneratorBuilder
{
    public ICodeGenerator Build()
        => new GLSLGenerator();
}
