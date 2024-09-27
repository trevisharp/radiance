/* Author:  Leonardo Trevisan Silio
 * Date:    27/09/2024
 */
using System.Collections.Generic;

namespace Radiance.Buffers;

using Contexts;
using OpenGL4;

/// <summary>
/// A global object to manage Buffers.
/// </summary>
public static class BufferManager
{
    /// <summary>
    /// Get or Set the Current Buffer Context Builder, the default is the OpenGL4 Buffer Context Builder.
    /// </summary>
    public static BufferContextBuilder BufferContextBuilder { get; set; } = new OpenGL4BufferContextBuilder();

    static int frameCount = 0;
    static readonly List<Buffer> buffers = [];
    public static void RegisterFrame()
    {
        frameCount++;
    }


}