using System.Collections.Generic;
using OpenTK.Compute.OpenCL;
using Radiance;
using static Radiance.Utils;

var shipRender = render((px) =>
{
    pos = 5 * pos + (px, height / 2, 0);
    color = red;
    fill();
});

var waveRender = render((px, py, size) =>
{
    pos = 34 * size * pos + (px, py, 0);
    color = white;
    draw();
});

float shipSpeed = 4f;
float shipPosition = 0f;
Window.OnRender += () =>
    shipRender(Polygons.Circle, shipPosition);

var clkFrame = new Clock();
var clkWave = new Clock();
List<Clock> waveClocks = [];
Window.OnFrame += () =>
{
    shipPosition += shipSpeed * clkFrame.Time;
    shipSpeed += 3 * clkFrame.Time;
    clkFrame.Reset();

    if (clkWave.Time < 0.2)
        return;
    clkWave.Reset();

    var clk = new Clock();
    waveClocks.Add(clk);
    var origin = shipPosition;
    Window.OnRender += () =>
        waveRender(Polygons.Circle, origin, Window.Height / 2, clk.Time);
};

Window.OnKeyDown += (k, m) =>
{
    if (k == Input.Space)
    {
        foreach (var clk in waveClocks)
            clk.ToogleFreeze();
        clkFrame.ToogleFreeze();
        clkWave.ToogleFreeze();
    }
};

Window.CloseOn(Input.Escape);
Window.Open();