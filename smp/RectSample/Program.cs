using System;
using System.Collections.Generic;
using System.Drawing;

using Duck;
using static Duck.Vector;

Graphics g = null;

DateTime dt = DateTime.Now;

int N = 1000;
Queue<DateTime> queue = new Queue<DateTime>();
DateTime newer = DateTime.Now;
DateTime older = DateTime.Now;

float x = 0;
float y = 0;
float sx = .1f;
float sy = .1f;

Window.OnLoad += delegate
{
    g = Graphics.New();
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

        if (x < -.9f)
            sx = .1f;
        else if (x > .9f)
            sx = -.1f;

        if (y < -.9f)
            sy = .1f;
        else if (y > .9f)
            sy = -.1f;
    }

    g.Clear(Color.Black);
    
    g.FillPolygon(
        Color.Orange,
        (x, y) + .1f * (- i - j),
        (x, y) + .1f * (+ i - j),
        (x, y) + .1f * (+ i + j),
        (x, y) + .1f * (- i + j)
    );

    g.DrawPolygon(
        Color.LightBlue,
        (x, y) + .1f * (- i - j),
        (x, y) + .1f * (+ i - j),
        (x, y) + .1f * (+ i + j),
        (x, y) + .1f * (- i + j)
    );
};

Window.OnKeyDown += e =>
{
    if (e == Input.Escape)
        Window.Close();
};

Window.Open();