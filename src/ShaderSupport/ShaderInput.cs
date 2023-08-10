/* Author:  Leonardo Trevisan Silio
 * Date:    10/08/2023
 */
namespace Radiance.ShaderSupport;

/// <summary>
/// Represents a Input for a Shader Implementation.
/// </summary>
public abstract class ShaderInput : ShaderDependence
{
    public abstract object Value { get; }
}