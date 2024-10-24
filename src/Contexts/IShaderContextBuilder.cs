/* Author:  Leonardo Trevisan Silio
 * Date:    24/10/2024
 */
namespace Radiance.Contexts;

/// <summary>
/// A Builder for a ShaderContext
/// </summary>
public interface IShaderContextBuilder
{
    /// <summary>
    /// Build a ShaderContext.
    /// </summary>
    ShaderContext Build();
}