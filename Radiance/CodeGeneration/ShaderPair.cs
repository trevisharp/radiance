/* Author:  Leonardo Trevisan Silio
 * Date:    25/09/2024
 */
using System;

namespace Radiance.CodeGeneration;

/// <summary>
/// Represents a pair of vertex and fragment shaders.
/// </summary>
public record ShaderPair(
    ShaderCode VertexShader,
    ShaderCode FragmentShader,
    Action? InitialConfiguration
);