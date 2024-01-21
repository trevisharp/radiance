/* Author:  Leonardo Trevisan Silio
 * Date:    14/08/2023
 */
namespace Radiance.Shaders;

/// <summary>
/// Represents a Type of Dependece used in a Shader Implementation.
/// </summary>
public enum ShaderDependenceType
{
    None = 0,
    Uniform = 1,
    Variable = 2,
    CustomData = 4,
    Texture = 8
}