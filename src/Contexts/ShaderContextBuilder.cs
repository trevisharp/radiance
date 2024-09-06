/* Author:  Leonardo Trevisan Silio
 * Date:    06/09/2024
 */
namespace Radiance.Contexts;

/// <summary>
/// A Builder for a ShaderContext
/// </summary>
public abstract class ShaderContextBuilder
{
    public abstract ShaderContext Build();
}