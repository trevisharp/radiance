using Radiance;
using static Radiance.Utils;

using System.Collections.Generic;

var shipRender = render((dx) =>
{
    zoom(5);
    move(dx, height / 2);
    color = red;
    fill();
});

var waveRender = render((dx, dy, size) =>
{
    zoom(60 * size);
    move(dx, dy);
    color = white;
    draw();
});

float shipSpeed = 4f;
float shipAcceleration = 4f;
float shipPosition = 0f;
Window.OnRender += () =>
    shipRender(Polygons.Circle, shipPosition);

var lastFrame = Clock.Shared.Time;
Window.OnFrame += () =>
{
    var newFrame = Clock.Shared.Time;
    var dt = newFrame - lastFrame;
    lastFrame = newFrame;

    shipSpeed += shipAcceleration * dt;
    shipPosition += shipSpeed * dt;
};

var clkWave = new Clock();
List<Clock> waveClocks = [];
Window.OnFrame += () =>
{
    if (clkWave.Time < 1)
        return;
    clkWave.Reset();

    var clk = new Clock();
    waveClocks.Add(clk);
    var origin = shipPosition;
    Window.OnRender += () =>
        waveRender(Polygons.Circle, origin, Window.Height / 2, clk.Time);
};

Window.OnKeyDown += (key, mod) =>
{
    if (key != Input.Space)
        return;
    
    foreach (var clk in waveClocks)
        clk.ToogleFreeze();
    Clock.Shared.ToogleFreeze();
    clkWave.ToogleFreeze();
};

Window.CloseOn(Input.Escape);
Window.Open();