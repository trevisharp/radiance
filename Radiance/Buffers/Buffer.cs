/* Author:  Leonardo Trevisan Silio
 * Date:    06/11/2024
 */
namespace Radiance.Buffers;

/// <summary>
/// Represents a GPU Buffer Data.
/// </summary>
public class Buffer
{
    /// <summary>
    /// Get the Id of the Buffer.
    /// </summary>
    public int? BufferId { get; set; } = null;

    /// <summary>
    /// Get or Set if the buffer change a lot.
    /// </summary>
    public bool DynamicDraw { get; set; } = false;

    /// <summary>
    /// Get or Set the Current data on the buffer.
    /// </summary>
    public IBufferedData? CurrentData { get; set; } = null;

    /// <summary>
    /// Create a buffer from a buffered data
    /// </summary>
    public static Buffer From(IBufferedData data)
    {
        return new Buffer {
            BufferId = null,
            CurrentData = data,
            DynamicDraw = false
        };
    }
}