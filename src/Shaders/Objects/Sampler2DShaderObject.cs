/* Author:  Leonardo Trevisan Silio
 * Date:    25/01/2023
 */
using System.Collections.Generic;

namespace Radiance.Shaders.Objects;

using Data;
using Dependencies;

/// <summary>
/// Represents a Sampler2D used by textures in OpenGL.
/// </summary>
public class Sampler2DShaderObject : ShaderObject
{
    public Sampler2DShaderObject(
        string value, ShaderOrigin origin,
        IEnumerable<ShaderDependence> deps
        ) : base(ShaderType.Vec3, value, origin, deps) { }
    
    public static implicit operator Sampler2DShaderObject(Texture texture)
    {
        var dep = new TextureDependence();
    }
}