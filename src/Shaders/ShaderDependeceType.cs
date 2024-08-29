/* Author:  Leonardo Trevisan Silio
 * Date:    22/01/2024
 */
namespace Radiance.Shaders;

/// <summary>
/// Represents a Type of Dependence used in a Shader Implementation.
/// </summary>
public enum ShaderDependenceType
{
    None = 0,
    Uniform = 1,
    Variable = 2,
    CustomData = 4,
    Texture = 8,
    Expression = 16
}