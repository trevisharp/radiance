/* Author:  Leonardo Trevisan Silio
 * Date:    09/10/2024
 */
namespace Radiance.Contexts;

using Buffers;
using Shaders;

/// <summary>
/// A invoker to initialize and run a shader.
/// </summary>
public interface IShaderInvoker
{
    /// <summary>
    /// Start to use a Data.
    /// </summary>
    void Use(IPolygon data);

    /// <summary>
    /// Draw Arrays in the selected buffer.
    /// </summary>
    void Draw(PrimitiveType primitiveType, IBufferedData data);
    
    /// <summary>
    /// Create and associeate the context to a program.
    /// </summary>
    void CreateProgram(ShaderPair pair, bool verbose = false);

    /// <summary>
    /// An optinional configuration method that will called once Between Use and UseProgram.
    /// And after other configurations based on shader dependeces.
    /// </summary>
    void Configure();

    /// <summary>
    /// Use the associated specific Program.
    /// </summary>
    void UseProgram();
}