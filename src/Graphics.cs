/* Author:  Leonardo Trevisan Silio
 * Date:    02/08/2023
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

    public static Graphics New()
        => new Graphics();
        
    public static Graphics New(int widht, int height)
        => new Graphics(widht, height);
    
    private int program = 0;
    private int vertexObject = 0;
    private bool disposed = false;

    private string vertexShaderSource = string.Empty;
    private string fragmentShaderSource = string.Empty;

    private int width = 0;
    private int height = 0;

    private Graphics(
        int width = 800,
        int height = 640,
        string vertexShaderSource = null,
        string fragmentShaderSource = null
    )
    {
        this.vertexShaderSource = vertexShaderSource ??
        """
        #version 330 core
        layout (location = 0) in vec3 aPosition;

        void main()
        {
            gl_Position = vec4(aPosition, 1.0);
        }
        """;

        this.fragmentShaderSource = fragmentShaderSource ??
        """
        #version 330 core
        out vec4 FragColor;
        uniform  vec4 ourColor;

        void main()
        {
            FragColor = ourColor;
        } 
        """;

        this.width = width;
        this.height = height;

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

        GL.VertexAttribPointer(0, 3,
            VertexAttribPointerType.Float, 
            false, 3 * sizeof(float), 0
        );
        GL.EnableVertexAttribArray(0);
        
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
        var colorCode = GL.GetUniformLocation(program, "ourColor");
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
        var colorCode = GL.GetUniformLocation(program, "ourColor");
        GL.Uniform4(colorCode, color.R / 255f, color.G / 255f, color.B / 255f, 1.0f);

        GL.BindVertexArray(vertexObject);
        GL.DrawArrays(PrimitiveType.LineLoop, 0, pts.Length);
    }

    private float[] toArr(Vertex[] pts, bool loop)
    {
        if (pts is null)
            return new float[0];

        int size = 3 * pts.Length + (loop ? 3 : 0);
        float[] vertices = new float[size];

        int index = 0;
        for (int i = 0; i < pts.Length; i++, index += 3)
        {
            var pt = pts[i];
            transformBasedOnWindowSize(pt, vertices, index);
        }
        
        if (!loop)
            return vertices;

        
        var first = pts[0];
        transformBasedOnWindowSize(first, vertices, index);

        return vertices;
    }

    private void transformBasedOnWindowSize(Vertex pt, float[] data, int offset)
    {
        data[offset + 0] = 2 * (pt.x / width) - 1;
        data[offset + 1] = 2 * (pt.y / height) - 1;
        data[offset + 2] = pt.z;
    }
}