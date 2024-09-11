/* Author:  Leonardo Trevisan Silio
 * Date:    06/09/2024
 */
namespace Radiance.Managers;

using Primitives;

/// <summary>
/// Represents the data and state of a shader program.
/// </summary>
public abstract class ShaderManager
{
    /// <summary>
    /// Set a uniform with a name to a specific value.
    /// </summary>
    public abstract void SetFloat(string name, float value);

    /// <summary>
    /// Set a image uniform with a name to a specific value.
    /// </summary>
    public abstract void SetTextureData(string name, Texture texture);
    
    /// <summary>
    /// Create a Resource from a Polygon.
    /// </summary>
    public abstract void CreateResources(Polygon poly);

    /// <summary>
    /// Start to use a Polygon.
    /// </summary>
    public abstract void Use(Polygon poly);
}