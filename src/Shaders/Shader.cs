/* Author:  Leonardo Trevisan Silio
 * Date:    11/09/2024
 */
using System;

namespace Radiance.Shaders;

/// <summary>
/// Represents a code and a setup configuration of a shader.
/// </summary>
public sealed record Shader(
    string Code, int Hash, Action? Setup
);