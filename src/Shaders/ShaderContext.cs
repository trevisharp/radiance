/* Author:  Leonardo Trevisan Silio
 * Date:    24/01/2024
 */
using StbImageSharp;
using OpenTK.Graphics.OpenGL4;

namespace Radiance.Shaders;

/// <summary>
/// Represents the state of shader generation.
/// </summary>
public class ShaderContext
{
    public int Program { get; set; }
    
    public void SetUniformFloat(string name, float value)
    {
        var code = GL.GetUniformLocation(Program, name);
        GL.Uniform1(code, value);
    }
}