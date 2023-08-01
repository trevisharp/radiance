using System;
using System.Drawing;
using System.Linq;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace WorldEngine;

public class Graphics : IDisposable
{
    private static int bufferObject = int.MinValue;

    public static Graphics New()
        => new Graphics();
    
    private int program = 0;
    private int vertexObject = 0;
    private bool disposed = false;
    private string vertexShaderSource = string.Empty;
    private string fragmentShaderSource = string.Empty;

    private Graphics(
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

    public void FillPolygon(Color color, params PointF[] pts)
    {
        GL.UseProgram(program);
        
        var query =
            from pt in pts
            select new float[]
            {
                pt.X, pt.Y, 0
            };
        
        var first = query.FirstOrDefault();

        var vertices = 
            query
            .Append(first)
            .SelectMany(x => x)
            .ToArray();
        
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
    
    public void DrawPolygon(Color color, params PointF[] pts)
    {
        GL.UseProgram(program);

        var query =
            from pt in pts
            select new float[]
            {
                pt.X, pt.Y, 0
            };

        var vertices = 
            query
            .SelectMany(x => x)
            .ToArray();
        
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
}