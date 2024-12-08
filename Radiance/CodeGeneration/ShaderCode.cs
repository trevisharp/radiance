/* Author:  Leonardo Trevisan Silio
 * Date:    11/09/2024
 */
using System;

namespace Radiance.CodeGeneration;

/// <summary>
/// Represents a code and a setup configuration of a shader.
/// </summary>
public sealed record ShaderCode(
    string Code, int Hash, Action? Setup
);