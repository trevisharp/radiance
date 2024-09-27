/* Author:  Leonardo Trevisan Silio
 * Date:    25/09/2024
 */
using System;

namespace Radiance.Shaders;

/// <summary>
/// Represents a pair of vertex and fragment shaders.
/// </summary>
public record ShaderPair(
    Shader VertexShader,
    Shader FragmentShader,
    Action? InitialConfiguration
);