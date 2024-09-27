/* Author:  Leonardo Trevisan Silio
 * Date:    27/09/2024
 */
using System.Collections.Generic;

namespace Radiance.Buffers;

using Buffers;
using Primitives;

/// <summary>
/// A global object to manage Buffers.
/// </summary>
public static class BufferManager
{
    static int frameCount = 0;
    static readonly List<Buffer> buffers = [];
    public static void RegisterFrame()
    {
        frameCount++;
    }

    
}