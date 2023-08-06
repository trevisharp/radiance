/* Author:  Leonardo Trevisan Silio
 * Date:    06/08/2023
 */
using System;
using System.Drawing;
using System.Linq;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace DuckGL;

/// <summary>
/// Provide access to a OpenGL simplifications based on OpenTK library.
/// </summary>
public class Graphics : IDisposable
{
    private static int bufferObject = int.MinValue;
    
    private int program = 0;
    private int vertexObject = 0;
    private bool disposed = false;

    private string vertexShaderSource = string.Empty;
    private string fragmentShaderSource = string.Empty;

    private int width = 0;
    private int height = 0;

    private int[] layoutInfo;

    internal Graphics(
        int width,
        int height,
        string vertexShaderSource,
        string fragmentShaderSource,
        int[] layoutInfo
    )
    {
        this.vertexShaderSource = vertexShaderSource;
        this.fragmentShaderSource = fragmentShaderSource;

        this.width = width;
        this.height = height;

        this.layoutInfo = layoutInfo;

        load();
    }

    static Graphics()
    { 
        bufferObject = GL.GenBuffer();
        GL.BindBuffer(
            BufferTarget.ArrayBuffer, 
            bufferObject
        );
    }

    public void Dispose()
    {
        if (disposed)
            return;
        disposed = true;

        unload();
    }

    private void load()
    {
        vertexObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexObject);

        int stride = layoutInfo.Sum();
        int offset = 0;
        for (int i = 0; i < layoutInfo.Length; i++)
        {
            GL.VertexAttribPointer(i,
                layoutInfo[i],
                VertexAttribPointerType.Float, 
                false, 
                stride * sizeof(float),
                offset * sizeof(float)
            );
            GL.EnableVertexAttribArray(i);

            offset += layoutInfo[i];
        }
        
        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        
        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);

        program = GL.CreateProgram();
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        GL.LinkProgram(program);

        GL.DetachShader(program, vertexShader);
        GL.DetachShader(program, fragmentShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);
    }

    private void unload()
    { 
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);

        GL.DeleteBuffer(bufferObject);
        GL.DeleteVertexArray(vertexObject);
        GL.DeleteProgram(program);
    }

    public void Clear(Color color)
    {
        GL.ClearColor(
            color.R / 255f,
            color.G / 255f,
            color.B / 255f,
            color.A / 255f
        );
        GL.Clear(ClearBufferMask.ColorBufferBit);
    }

    public void FillPolygon(Color color, params Vertex[] pts)
    {
        GL.UseProgram(program);

        float[] vertices = toArr(pts, true);
        
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            vertices.Length * sizeof(float), 
            vertices, 
            BufferUsageHint.DynamicDraw
        );
        
        var colorCode = GL.GetUniformLocation(program, "uniform0");
        GL.Uniform4(colorCode, color.R / 255f, color.G / 255f, color.B / 255f, 1.0f);

        GL.BindVertexArray(vertexObject);
        GL.DrawArrays(PrimitiveType.TriangleStrip, 0, pts.Length + 1);
    }
    
    public void DrawPolygon(Color color, params Vertex[] pts)
    {
        GL.UseProgram(program);

        float[] vertices = toArr(pts, false);
        
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            vertices.Length * sizeof(float), 
            vertices, 
            BufferUsageHint.DynamicDraw
        );
        
        var colorCode = GL.GetUniformLocation(program, "uniform0");
        GL.Uniform4(colorCode, color.R / 255f, color.G / 255f, color.B / 255f, 1.0f);

        GL.BindVertexArray(vertexObject);
        GL.DrawArrays(PrimitiveType.LineLoop, 0, pts.Length);
    }

    public void FillPolygon(params Vertex[] pts)
    {
        GL.UseProgram(program);

        float[] vertices = toArr(pts, true);
        
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            vertices.Length * sizeof(float), 
            vertices, 
            BufferUsageHint.DynamicDraw
        );

        GL.BindVertexArray(vertexObject);
        GL.DrawArrays(PrimitiveType.TriangleStrip, 0, pts.Length + 1);
    }
    
    public void DrawPolygon(params Vertex[] pts)
    {
        GL.UseProgram(program);

        float[] vertices = toArr(pts, false);
        
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            vertices.Length * sizeof(float), 
            vertices, 
            BufferUsageHint.DynamicDraw
        );

        GL.BindVertexArray(vertexObject);
        GL.DrawArrays(PrimitiveType.LineLoop, 0, pts.Length);
    }

    public void FillPolygon(params ColoredVertex[] pts)
    {
        GL.UseProgram(program);

        float[] vertices = toArr(pts, true);
        
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            vertices.Length * sizeof(float), 
            vertices, 
            BufferUsageHint.DynamicDraw
        );

        GL.BindVertexArray(vertexObject);
        GL.DrawArrays(PrimitiveType.TriangleFan, 0, pts.Length + 1);
    }
    
    public void DrawPolygon(params ColoredVertex[] pts)
    {
        GL.UseProgram(program);

        float[] vertices = toArr(pts, false);
        
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            vertices.Length * sizeof(float), 
            vertices, 
            BufferUsageHint.DynamicDraw
        );

        GL.BindVertexArray(vertexObject);
        GL.DrawArrays(PrimitiveType.LineLoop, 0, pts.Length);
    }

    private float[] toArr(ColoredVertex[] pts, bool loop)
    {
        if (pts is null)
            return new float[0];

        int size = 7 * pts.Length + (loop ? 7 : 0);
        float[] vertices = new float[size];

        int offset = 0;
        for (int i = 0; i < pts.Length; i++, offset += 7)
        {
            var pt = pts[i];
            transformBasedOnWindowSize(pt, vertices, offset);

            var color = pt.Color;
            transformColor(color, vertices, offset + 3);
        }
        
        if (!loop)
            return vertices;
        
        var first = pts[0];
        transformBasedOnWindowSize(first, vertices, offset);

        var firstColor = first.Color;
        transformColor(firstColor, vertices, offset + 3);

        return vertices;
    }

    private float[] toArr(Vertex[] pts, bool loop)
    {
        if (pts is null)
            return new float[0];

        int size = 3 * pts.Length + (loop ? 3 : 0);
        float[] vertices = new float[size];

        int offset = 0;
        for (int i = 0; i < pts.Length; i++, offset += 3)
        {
            var pt = pts[i];
            transformBasedOnWindowSize(pt, vertices, offset);
        }
        
        if (!loop)
            return vertices;

        
        var first = pts[0];
        transformBasedOnWindowSize(first, vertices, offset);

        return vertices;
    }

    private void transformColor(Color color, float[] data, int offset)
    {
        data[offset + 0] = color.R / 255f;
        data[offset + 1] = color.G / 255f;
        data[offset + 2] = color.B / 255f;
        data[offset + 3] = color.A / 255f;
    }

    private void transformBasedOnWindowSize(Vertex pt, float[] data, int offset)
    {
        data[offset + 0] = 2 * (pt.x / width) - 1;
        data[offset + 1] = 1 - 2 * (pt.y / height);
        data[offset + 2] = pt.z;
    }
}