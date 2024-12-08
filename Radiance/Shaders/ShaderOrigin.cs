/* Author:  Leonardo Trevisan Silio
 * Date:    23/01/2024
 */
namespace Radiance.Shaders;

/// <summary>
/// Enumerate the types of origins that a object can have.
/// </summary>
public enum ShaderOrigin : byte
{
    FragmentShader = 0b01,
    VertexShader = 0b10,
    Global = 0b11
}