/* Author:  Leonardo Trevisan Silio
 * Date:    24/10/2024
 */
namespace Radiance.OpenGL4;

using Contexts;

/// <summary>
/// A Builder for a ShaderContext.
/// </summary>
public class OpenGL4ShaderContextBuilder : IShaderContextBuilder
{
    public ShaderContext Build()
        => new OpenGL4ShaderContext();
}