/* Author:  Leonardo Trevisan Silio
 * Date:    27/09/2024
 */
namespace Radiance.Buffers;

/// <summary>
/// Represents a GPU Buffer Data.
/// </summary>
public partial class Buffer
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
    /// Get the frame count of the creation of this buffer.
    /// </summary>
    public int? FrameCreation { get; set; } = null;

    /// <summary>
    /// Get the count of times that data changes.
    /// </summary>
    public int? ChangeCount { get; set; } = null;

    /// <summary>
    /// Get the last frame when the buffer is used.
    /// </summary>
    public int? LastUsageFrame { get; set; } = null;

    /// <summary>
    /// Get the last frame when the data is changed inside the buffer.
    /// </summary>
    public int? LastChangedFrame { get; set; } = null;
}