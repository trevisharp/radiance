/* Author:  Leonardo Trevisan Silio
 * Date:    11/09/2024
 */
namespace Radiance.Shaders.CodeGeneration;

/// <summary>
/// Represents a pair of vertex and fragment shaders.
/// </summary>
public record ShaderPair(
    Shader VertexShader,
    Shader FragmentShader
);