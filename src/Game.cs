using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

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

        Graphics g = null;

        main.Load += delegate
        {
            g = new Graphics();
        };

        main.Unload += delegate
        {
            g.Dispose();
        };

        DateTime dt = DateTime.Now;

        int N = 1000;
        Queue<DateTime> queue = new Queue<DateTime>();
        DateTime older = DateTime.Now;

        float x = 0;
        float y = 0;

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

            g.Clear(Color.Black);
            
            g.FillPolygon(
                Color.Orange,
                new PointF(x - .1f, y - .1f),
                new PointF(x + .1f, y - .1f),
                new PointF(x + .1f, y + .1f),
                new PointF(x - .1f, y + .1f)
            );

            g.DrawPolygon(
                Color.LightBlue,
                new PointF(x - .1f, y - .1f),
                new PointF(x + .1f, y - .1f),
                new PointF(x + .1f, y + .1f),
                new PointF(x - .1f, y + .1f)
            );

            main.SwapBuffers();
        };

        main.UpdateFrame += e =>
        {
            if (main.KeyboardState.IsKeyDown(Keys.Escape))
            {
                main.Close();
            }
        };

        main.Run();
    }
}