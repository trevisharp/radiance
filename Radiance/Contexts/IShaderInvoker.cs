/* Author:  Leonardo Trevisan Silio
 * Date:    29/10/2024
 */
namespace Radiance.Contexts;

using Bufferings;
using CodeGeneration;

/// <summary>
/// A invoker to initialize and run a shader.
/// </summary>
public interface IShaderInvoker
{
    /// <summary>
    /// Configure the shader to use a set of arguments.
    /// </summary>
    void UseArgs(object[] args);

    /// <summary>
    /// Initial configure the shader to use a set of arguments.
    /// </summary>
    void InitArgs(object[] args);

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
    void FirstConfiguration();

    /// <summary>
    /// Use the associated specific Program.
    /// </summary>
    void UseProgram();
}