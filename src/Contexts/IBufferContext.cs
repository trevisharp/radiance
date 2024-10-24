/* Author:  Leonardo Trevisan Silio
 * Date:    24/10/2024
 */
namespace Radiance.Contexts;

/// <summary>
/// Represents the data and state of a GPU buffers.
/// </summary>
public interface IBufferContext
{
    /// <summary>
    /// Create a buffer and returns their Id.
    /// </summary>
    int Create();

    /// <summary>
    /// Bind a Buffer based on their Id.
    /// </summary>
    void Bind(int Id);

    /// <summary>
    /// Delete a Buffer based on their Id.
    /// </summary>
    void Delete(int Id);

    /// <summary>
    /// Set a data on buffer.
    /// </summary>
    void Store(float[] data, bool dynamicData);
}