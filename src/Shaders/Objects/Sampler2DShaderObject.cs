/* Author:  Leonardo Trevisan Silio
 * Date:    30/09/2023
 */
using System.Collections.Generic;

namespace Radiance.Shaders.Objects;

/// <summary>
/// Represents a Sampler2D used by textures in OpenGL.
/// </summary>
public class Sampler2DShaderObject(
    string value, ShaderOrigin origin,
    IEnumerable<ShaderDependence> deps
        ) : ShaderObject(ShaderType.Sampler, value, origin, deps)
{
    public FloatShaderObject width {
        get
        {

        }
    }
    
    public FloatShaderObject height {
        get
        {

        }
    }
}