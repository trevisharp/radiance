/* Author:  Leonardo Trevisan Silio
 * Date:    30/09/2023
 */
using System.Linq;
using System.Collections.Generic;

namespace Radiance.Shaders.Objects;

using Exceptions;
using Dependencies;

/// <summary>
/// Represents a Sampler2D used by textures in OpenGL.
/// </summary>
public class Sampler2DShaderObject(
    string value, ShaderOrigin origin,
    IEnumerable<ShaderDependence> deps
        ) : ShaderObject(ShaderType.Sampler, value, origin, deps)
{
    readonly TextureDependence mainDep = 
        deps.FirstOrDefault(dep => dep is TextureDependence) as TextureDependence
        ?? throw new MissingTextureDependenceException();
    
    TextureDataDependence? dataDep = null;
    TextureDataDependence GetDataDep()
    {
        dataDep ??= new TextureDataDependence(mainDep);
        return dataDep;
    }

    public FloatShaderObject width {
        get
        {
            var dep = GetDataDep();
            return new FloatShaderObject($"{dep.name}.x", ShaderOrigin.Global, [ dep ]);
        }
    }
    
    public FloatShaderObject height {
        get
        {
            var dep = GetDataDep();
            return new FloatShaderObject($"{dep.name}.y", ShaderOrigin.Global, [ dep ]);
        }
    }
}