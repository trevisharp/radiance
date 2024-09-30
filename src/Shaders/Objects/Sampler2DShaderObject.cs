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

    /// <summary>
    /// The width of input image.
    /// </summary>
    public FloatShaderObject width
    {
        get
        {
            var dep = GetDataDep();
            return new FloatShaderObject($"{dep.name}.x", ShaderOrigin.Global, [ dep ]);
        }
    }
    
    /// <summary>
    /// The height of input image.
    /// </summary>
    public FloatShaderObject height
    {
        get
        {
            var dep = GetDataDep();
            return new FloatShaderObject($"{dep.name}.y", ShaderOrigin.Global, [ dep ]);
        }
    }

    /// <summary>
    /// The relationship between the screen width and the current image width.
    /// On expression like texture(im, x, y) multiply x per xratio to get fullscreen image.
    /// </summary>
    public FloatShaderObject xratio => width / Utils.width;

    /// <summary>
    /// The relationship between the screen height and the current image height.
    /// On expression like texture(im, x, y) multiply y per yratio to get fullscreen image.
    /// </summary>
    public FloatShaderObject yratio => height / Utils.height;
}