using System;
using System.Linq;

using Radiance;
using static Radiance.Utils;

var shipRender = render((px) =>
{
    pos *= 5;
    pos += (px, height / 2, 0);
    color = red;
    fill();
});

var waveRender = render((px, py, init) =>
{
    var size = 34 * (t - init);
    pos *= size;
    pos += (px, py, 0);
    color = white;
    draw();
});

float shipSpeed = 4f;
float shipPosition = 0f;
Window.OnLoad += () 
    => shipPosition = 0;

Window.OnRender += () =>
    shipRender(Circle, shipPosition);

var lastUpdate = DateTime.Now;
var last = Time;
var now = Time;
Window.OnFrame += () =>
{
    now = Time;
    var passed = now - last;
    shipPosition += shipSpeed * passed;
    last = now;

    shipSpeed += 3 * passed;

    var frameTime = DateTime.Now - lastUpdate;
    if (frameTime.TotalMilliseconds < 200)
        return;
    lastUpdate = DateTime.Now;

    float px = shipPosition;
    float py = Window.Height / 2;
    float secs = Time;
    Window.OnRender += () =>
        waveRender(Circle, px, py, secs);
};

Window.CloseOn(Input.Escape);

Window.Open();