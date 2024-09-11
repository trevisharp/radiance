/* Author:  Leonardo Trevisan Silio
 * Date:    06/09/2024
 */
namespace Radiance.OpenGL4;

using Managers;

/// <summary>
/// A Builder for a ShaderContext.
/// </summary>
public class OpenGL4ShaderContextBuilder : ShaderManagerBuilder
{
    public override ShaderManager Build()
        => new OpenGL4ShaderContext();
}