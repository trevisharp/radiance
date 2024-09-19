/* Author:  Leonardo Trevisan Silio
 * Date:    25/01/2023
 */
using System.Collections.Generic;

namespace Radiance.Shaders.Objects;

/// <summary>
/// Represents a Sampler2D used by textures in OpenGL.
/// </summary>
public class Sampler2DShaderObject : ShaderObject
{
    public Sampler2DShaderObject(
        string value, ShaderOrigin origin,
        IEnumerable<ShaderDependence> deps
        ) : base(ShaderType.Sampler, value, origin, deps) { }
}