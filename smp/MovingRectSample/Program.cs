using System;
using System.Collections.Generic;
using System.Drawing;

using DuckGL;
using static DuckGL.Vector;

Graphics g = null;

DateTime dt = DateTime.Now;

int N = 1000;
Queue<DateTime> queue = new Queue<DateTime>();
DateTime newer = DateTime.Now;
DateTime older = DateTime.Now;

float x = 0;
float y = 0;
float baseSpeed = 100;
float sx = baseSpeed;
float sy = baseSpeed;

Window.OnLoad += delegate
{
    g = Graphics.New(Window.Width, Window.Height);

    x = Window.Width / 2 - 25;
    y = Window.Height / 2 - 25;
};

Window.OnUnload += delegate
{
    g.Dispose();
};

Window.OnRender += delegate
{
    newer = DateTime.Now;
    queue.Enqueue(newer);

    if (queue.Count > N - 1)
    {
        older = queue.Dequeue();
        var delta = newer - older;
        var fps = (int)(N / delta.TotalSeconds);
        Console.WriteLine($"fps: {fps}");

        x += sx / fps;
        y += sy / fps;

        if (x < 50)
            sx = baseSpeed;
        else if (x > Window.Width - 50)
            sx = -baseSpeed;

        if (y < 50)
            sy = baseSpeed;
        else if (y > Window.Height - 50)
            sy = -baseSpeed;
    }

    g.Clear(Color.Black);
    
    g.FillPolygon(
        Color.Orange,
        (x, y) + 50 * (- i - j),
        (x, y) + 50 * (+ i - j),
        (x, y) + 50 * (+ i + j),
        (x, y) + 50 * (- i + j)
    );

    g.DrawPolygon(
        Color.LightBlue,
        (x, y) + 50 * (- i - j),
        (x, y) + 50 * (+ i - j),
        (x, y) + 50 * (+ i + j),
        (x, y) + 50 * (- i + j)
    );
};

Window.OnKeyDown += e =>
{
    if (e == Input.Escape)
        Window.Close();
};

Window.Open();