/* Author:  Leonardo Trevisan Silio
 * Date:    06/11/2024
 */
using System;
using System.Collections.Concurrent;

namespace Radiance.Bufferings;

/// <summary>
/// Represents a GPU Buffer Data.
/// </summary>
public class Buffer
{
    static readonly ConcurrentQueue<(int Id, int Generation)> releasedIds = new();

    /// <summary>
    /// Take a released buffer id waiting to be deleted on the GPU.
    /// </summary>
    public static bool TryTakeReleased(out int id, out int generation)
    {
        var taken = releasedIds.TryDequeue(out var released);
        (id, generation) = released;
        return taken;
    }

    /// <summary>
    /// Get or Set the graphics context generation where the buffer was created.
    /// </summary>
    public int Generation { get; set; } = 0;

    /// <summary>
    /// Release the GPU buffer. The deletion happens on the window thread, on the next frame.
    /// Drawing the data again uploads it to a new buffer.
    /// </summary>
    public void Release()
    {
        var id = BufferId;
        if (id is null)
            return;

        BufferId = null;
        releasedIds.Enqueue((id.Value, Generation));
        GC.SuppressFinalize(this);
    }

    ~Buffer()
    {
        if (BufferId is int id)
            releasedIds.Enqueue((id, Generation));
    }

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
        => From(data, false);

    /// <summary>
    /// Create a buffer for a data.
    /// </summary>
    public static Buffer From(IBufferedData data, bool dynamicDraw)
    {
        return new Buffer {
            BufferId = null,
            CurrentData = data,
            DynamicDraw = dynamicDraw
        };
    }
}