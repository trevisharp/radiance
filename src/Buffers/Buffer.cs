/* Author:  Leonardo Trevisan Silio
 * Date:    27/09/2024
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
    /// Get Or Set if the buffer change a lot.
    /// </summary>
    public bool DynamicDraw { get; set; } = false;

    /// <summary>
    /// Get or Set the Current data on the buffer.
    /// </summary>
    public IBufferedData? CurrentData { get; set; } = null;

    /// <summary>
    /// The count of float values per point on buffer.
    /// </summary>
    public int LayoutSize { get; set; } = 0;

    /// <summary>
    /// Get the last frame when the buffer is used.
    /// </summary>
    public int? LastFrameUsage { get; set; } = null;

    /// <summary>
    /// Get the last frame when the data is changed inside the buffer.
    /// </summary>
    public int? LastFrameChanged { get; set; } = null;
}