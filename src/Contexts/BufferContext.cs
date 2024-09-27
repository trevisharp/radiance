/* Author:  Leonardo Trevisan Silio
 * Date:    27/09/2024
 */
namespace Radiance.Contexts;

/// <summary>
/// Represents the data and state of a GPU buffers.
/// </summary>
public abstract class BufferContext
{
    /// <summary>
    /// Create a buffer and returns their Id.
    /// </summary>
    public abstract int Create();

    /// <summary>
    /// Bind a Buffer based on their Id.
    /// </summary>
    public abstract void Bind(int Id);

    /// <summary>
    /// Delete a Buffer based on their Id.
    /// </summary>
    public abstract void Delete(int Id);

    /// <summary>
    /// Set a data on buffer.
    /// </summary>
    public abstract void Store(float[] data, bool dynamicData);
}