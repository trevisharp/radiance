/* Author:  Leonardo Trevisan Silio
 * Date:    05/09/2024
 */
using System.Threading;
using System.Collections.Generic;

namespace Radiance.Contexts;

using Primitives;

/// <summary>
/// A Thread-Safe global context to access current frame rendering data.
/// </summary>
public class FrameContext
{
    static readonly Dictionary<int, FrameContext> threadMap = [];

    static int GetCurrentThreadId()
    {
        var crr = Thread.CurrentThread;
        var id  = crr.ManagedThreadId;
        return id;
    }

    /// <summary>
    /// Open a new context for this thread.
    /// </summary>
    public static FrameContext OpenContext()
    {
        CloseContext();

        var opened = new FrameContext();
        var id = GetCurrentThreadId();
        threadMap.Add(id, opened);

        return opened;
    }

    /// <summary>
    /// Close the context for this thread.
    /// </summary>
    public static void CloseContext()
    {
        if (GetContext() is null)
            return;

        var id = GetCurrentThreadId();
        threadMap.Remove(id);
    }

    /// <summary>
    /// Get the opened context for this thread or null if it is closed.
    /// </summary>
    public static FrameContext? GetContext()
    {
        var id = GetCurrentThreadId();
        return threadMap.TryGetValue(id, out var ctx)
            ? ctx : null;
    }

    
}