/* Author:  Leonardo Trevisan Silio
 * Date:    13/12/2024
 */
#pragma warning disable CS8981
#pragma warning disable IDE1006
using System.Collections.Generic;

namespace Radiance;

using Shaders;
using Shaders.Objects;

/// <summary>
/// Types like val, vec2, vec3, vec4 and img.
/// </summary>
public interface IAlias;

/// <summary>
/// Alias name for Float Shader Type
/// </summary>
public sealed class val(
    string str, 
    ShaderOrigin origin,
    IEnumerable<ShaderDependence> deps
) : FloatShaderObject(str, origin, deps), IAlias;

/// <summary>
/// Alias name for Vec2 Shader Type
/// </summary>
public sealed class vec2(
    string str, 
    ShaderOrigin origin,
    IEnumerable<ShaderDependence> deps
) : Vec2ShaderObject(str, origin, deps), IAlias;

/// <summary>
/// Alias name for Vec3 Shader Type
/// </summary>
public sealed class vec3(
    string str, 
    ShaderOrigin origin,
    IEnumerable<ShaderDependence> deps
) : Vec3ShaderObject(str, origin, deps), IAlias;

/// <summary>
/// Alias name for vec4 Shader Type
/// </summary>
public sealed class vec4(
    string str, 
    ShaderOrigin origin,
    IEnumerable<ShaderDependence> deps
) : Vec4ShaderObject(str, origin, deps), IAlias;

/// <summary>
/// Alias name for Sampler Shader Type
/// </summary>
public sealed class img(
    string str, 
    ShaderOrigin origin,
    IEnumerable<ShaderDependence> deps
) : Sampler2DShaderObject(str, origin, deps), IAlias;