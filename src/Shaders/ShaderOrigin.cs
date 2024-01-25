/* Author:  Leonardo Trevisan Silio
 * Date:    24/01/2024
 */
namespace Radiance.Shaders;

/// <summary>
/// Enumerate the types of origins that a object can have.
/// </summary>
public class ShaderOrigin(string name)
{
    public readonly string Name = name;

    public static readonly ShaderOrigin Global = new("global");
    public static readonly ShaderOrigin VertexShader = new("vertex");
    public static readonly ShaderOrigin FragmentShader = new("fragment");
}