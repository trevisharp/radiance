/* Author:  Leonardo Trevisan Silio
 * Date:    27/09/2024
 */
using System.Linq;
using System.Collections.Generic;

namespace Radiance.Buffers;

using OpenGL4;
using Contexts;
using Exceptions;

/// <summary>
/// A global object to manage Buffers.
/// </summary>
public static class BufferManager
{
    /// <summary>
    /// Get or Set the Current Buffer Context Builder, the default is the OpenGL4 Buffer Context Builder.
    /// </summary>
    public static IBufferContextBuilder BufferContextBuilder { get; set; } = new OpenGL4BufferContextBuilder();

    static int currentFrame = 0;
    static readonly List<Buffer> buffers = [];
    public static void RegisterFrame()
    {
        currentFrame++;
        var toDeleteBuffers = buffers
            .Where(buffer =>
            {
                var stopedTime = currentFrame - buffer.LastUsageFrame;
                if (stopedTime > 20)
                    return true;
                
                return false;
            })
            .ToArray();
        foreach (var buffer in toDeleteBuffers)
            DeleteBuffer(buffer, BufferContextBuilder.Build());
    }

    public static void Use(IBufferedData data)
    {
        var ctx = BufferContextBuilder.Build();
        
        var created = CreateBuffer(data, ctx);
        var buffer = data.Buffer
            ?? throw new UnbufferedDataExcetion();
        buffer.LastUsageFrame = currentFrame;
        
        BindBuffer(buffer, ctx);

        if (created)
            SetBufferData(data.GetBufferData(), buffer, ctx);
    }

    private static bool CreateBuffer(IBufferedData data, IBufferContext ctx)
    {
        if (data.Buffer is not null)
            return false;
        
        var id = ctx.Create();
        var buffer = new Buffer {
            BufferId = id,
            ChangeCount = 0,
            CurrentData = data,
            DynamicDraw = false,
            LastChangedFrame = null,
            FrameCreation = currentFrame,
            LastUsageFrame = null
        };
        buffers.Add(buffer);
        data.Buffer = buffer;
        return true;
    }

    private static void DeleteBuffer(Buffer buffer, IBufferContext ctx)
    {
        var data = buffer.CurrentData;
        if (data is not null)
            data.Buffer = null;

        buffers.Remove(buffer);

        var id = buffer.BufferId;
        if (id is null)
            return;

        ctx.Delete(id.Value);
    }

    private static void BindBuffer(Buffer buffer, IBufferContext ctx)
    {
        var id = buffer.BufferId;
        if (id is null)
            return;

        ctx.Bind(id.Value);
    }

    private static void SetBufferData(float[] data, Buffer buffer, IBufferContext ctx)
    {
        ctx.Store(data, buffer.DynamicDraw);
    }
}