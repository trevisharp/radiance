/* Author:  Leonardo Trevisan Silio
 * Date:    06/09/2024
 */
namespace Radiance.Contexts.OpenGL;

/// <summary>
/// A Builder for a ShaderContext.
/// </summary>
public class OpenGL4ShaderContextBuilder : ShaderContextBuilder
{
    public override ShaderContext Build()
        => new OpenGL4ShaderContext();
}