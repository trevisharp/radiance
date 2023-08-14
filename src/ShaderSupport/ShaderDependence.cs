/* Author:  Leonardo Trevisan Silio
 * Date:    10/08/2023
 */
namespace Radiance.ShaderSupport;

/// <summary>
/// Represents a Input for a Shader Implementation.
/// </summary>
public abstract class ShaderDependence
{
    public abstract object Value { get; }
    public abstract string GetHeader(params object[] args);
    public string Name { get; set; }
    public ShaderType Type { get; set; }
    public ShaderDependenceType DependenceType { get; set; }
}