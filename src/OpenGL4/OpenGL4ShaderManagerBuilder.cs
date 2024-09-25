/* Author:  Leonardo Trevisan Silio
 * Date:    06/09/2024
 */
namespace Radiance.OpenGL4;

using Contexts;

/// <summary>
/// A Builder for a ShaderContext.
/// </summary>
public class OpenGL4ShaderManagerBuilder : ShaderContextBuilder
{
    public override ShadeContext Build()
        => new OpenGL4ShaderContext();
}