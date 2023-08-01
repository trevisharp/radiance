using System;
using System.Collections.Generic;
using System.IO;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace WorldEngine;

public class Game
{
    public void Open()
    {
        using var main = new GameWindow(
            GameWindowSettings.Default,
            new NativeWindowSettings()
            {
                Size = (800, 600),
                WindowState = WindowState.Fullscreen
            }
        );

        float[] _vertices =
        {
            -0.5f, -0.5f, 0.0f, 
             0.5f, -0.5f, 0.0f, 
             0.0f,  0.5f, 0.0f,
            -1f, -0.6f, 0.0f, 
             0.6f, -0.6f, 0.0f, 
             0.0f,  -1f, 0.0f
        };
        int _vertexBufferObject = 0;
        int _vertexArrayObject = 0;
        int Handle = 0;

        main.Load += delegate
        {
            GL.ClearColor(0f, 0f, 0f, 1.0f);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(
                BufferTarget.ArrayBuffer, 
                _vertexBufferObject
            );
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                _vertices.Length * sizeof(float), 
                _vertices, 
                BufferUsageHint.StaticDraw
            );

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.VertexAttribPointer(0, 3, 
                VertexAttribPointerType.Float, 
                false, 
                3 * sizeof(float), 
                0
            );
            GL.EnableVertexAttribArray(0);
            
            var shaderSource = 
            """
            #version 330 core
            layout (location = 0) in vec3 aPosition;

            void main()
            {
                gl_Position = vec4(aPosition, 1.0);
            }
            """;
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, shaderSource);
            GL.CompileShader(vertexShader);
            
            shaderSource = 
            """
            #version 330 core
            out vec4 FragColor;

            uniform  vec4 ourColor;

            void main()
            {
                FragColor = ourColor;
            } 
            """;
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, shaderSource);
            GL.CompileShader(fragmentShader);

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            GL.LinkProgram(Handle);

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);
        };

        DateTime dt = DateTime.Now;

        int N = 1000;
        Queue<DateTime> queue = new Queue<DateTime>();
        DateTime older = DateTime.Now;

        main.RenderFrame += e =>
        {
            var newer = DateTime.Now;
            queue.Enqueue(newer);

            if (queue.Count > N - 1)
            {
                older = queue.Dequeue();

                var delta = newer - older;
                var fps = N / delta.TotalSeconds;
                Console.WriteLine($"{(int)fps} fps");
            }


            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(Handle);
            
            double timeValue = (DateTime.Now - dt).TotalSeconds;
            float greenValue = (float)Math.Sin(timeValue) / 2.0f + 0.5f;
            int vertexColorLocation = GL.GetUniformLocation(Handle, "ourColor");
            GL.Uniform4(vertexColorLocation, 0.0f, greenValue, 0.0f, 1.0f);

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            main.SwapBuffers();
        };

        main.UpdateFrame += e =>
        {
            if (main.KeyboardState.IsKeyDown(Keys.Escape))
            {
                main.Close();
            }
        };

        main.Unload += delegate
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
            GL.DeleteProgram(Handle);
        };

        main.Run();
    }
}