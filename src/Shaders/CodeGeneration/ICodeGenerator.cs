/* Author:  Leonardo Trevisan Silio
 * Date:    12/09/2024
 */
namespace Radiance.Shaders.CodeGeneration;

using Objects;
using Contexts;

public interface ICodeGenerator
{
    /// <summary>
    /// Recive a object for a vertex shader and 
    /// a object for pixel/fragment shader and a
    /// context and generate GPU code.
    /// </summary>
    ShaderPair GenerateShaders(
        Vec3ShaderObject vertObj,
        Vec4ShaderObject fragObj,
        ShaderContext ctx
    );
}