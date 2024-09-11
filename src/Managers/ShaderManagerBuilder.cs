/* Author:  Leonardo Trevisan Silio
 * Date:    06/09/2024
 */
namespace Radiance.Managers;

/// <summary>
/// A Builder for a ShaderContext
/// </summary>
public abstract class ShaderManagerBuilder
{
    public abstract ShaderManager Build();
}