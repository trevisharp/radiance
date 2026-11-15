/* Author:  Leonardo Trevisan Silio
 * Date:    16/11/2026
 */
namespace Radiance.Shaders;

/// <summary>
/// Represents a dependence that receives float values from render arguments.
/// </summary>
public interface IFloatArgumentDependence
{
    /// <summary>
    /// Set a component of the value received from a render argument.
    /// </summary>
    void SetArgument(int component, float value);
}
