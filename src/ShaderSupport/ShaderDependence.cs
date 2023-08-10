/* Author:  Leonardo Trevisan Silio
 * Date:    10/08/2023
 */
namespace Radiance.ShaderSupport;

/// <summary>
/// Represents a Dependece used in a Shader Implementation.
/// </summary>
public abstract class ShaderDependence
{
    public string Name { get; set; }
    public ShaderType Type { get; set; }
    public ShaderDependenceType DependenceType { get; set; }
}