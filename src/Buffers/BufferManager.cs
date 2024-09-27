/* Author:  Leonardo Trevisan Silio
 * Date:    27/09/2024
 */
using System;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL4;

namespace Radiance.Buffers;

using OpenGL4;
using Contexts;

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

    public static void Use(IBufferedData data)
    {
        CreateBuffer((Polygon)data);
        BindBuffer((Polygon)data);
        SetBufferData((Polygon)data);
    }

    
    private static void CreateBuffer(Polygon poly)
    {
        if (poly.BufferId is not null)
            return;
        
        var id = GL.GenBuffer();
        poly.BufferId = id;
    }

    private static void DeleteBuffer(int bufferId)
    {
        GL.DeleteBuffer(bufferId);
    }

    private static void BindBuffer(Polygon poly)
    {
        int bufferId = poly.BufferId ?? 
            throw new Exception("A unexpected behaviour ocurred on buffer creation/binding.");
        GL.BindBuffer(
            BufferTarget.ArrayBuffer, 
            bufferId
        );
    }

    private static void SetBufferData(Polygon poly)
    {
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            poly.Data.Length * sizeof(float), poly.Data,
            BufferUsageHint.DynamicDraw
        );
    }
}