using Radiance;
using static Radiance.Utils;

using System.Collections.Generic;

// var shipRender = render((dx) =>
// {
//     zoom(5);
//     move(dx, height / 2);
//     color = red;
//     fill();
// });

// var waveRender = render((dx, dy, size) =>
// {
//     zoom(60 * size);
//     move(dx, dy);
//     color = white;
//     draw();
// });

// float shipSpeed = 8f;
// float shipAcceleration = 2f;
// float shipPosition = 0f;
// Window.OnRender += () =>
//     shipRender(Polygons.Circle, shipPosition);

// var lastFrame = Clock.Shared.Time;
// Window.OnFrame += () =>
// {
//     var newFrame = Clock.Shared.Time;
//     var dt = newFrame - lastFrame;
//     lastFrame = newFrame;

//     shipSpeed += shipAcceleration * dt;
//     shipPosition += shipSpeed * dt;
// };

// var clkWave = new Clock();
// List<Clock> waveClocks = [];
// Window.OnFrame += () =>
// {
//     if (clkWave.Time < 0.5)
//         return;
//     clkWave.Reset();

//     var clk = new Clock();
//     waveClocks.Add(clk);
//     var origin = shipPosition;
//     Window.OnRender += () =>
//         waveRender(Polygons.Circle, origin, Window.Height / 2, clk.Time);
// };

// Window.OnKeyDown += (key, mod) =>
// {
//     if (key != Input.Space)
//         return;
    
//     foreach (var clk in waveClocks)
//         clk.ToogleFreeze();
//     Clock.Shared.ToogleFreeze();
//     clkWave.ToogleFreeze();
// };

const int N = 16;
var mr = render((dx, dy, r, g, b) => {
    zoom(50);
    rotate(t);
    move(
        0.6 * width * dx + 0.2 * width,
        0.6 * height * dy + 0.2 * height
    );
    color = (r, g, b, 1);
    fill();

    pos = (pos.x, pos.y, pos.z + 1);
    color = (r * 0.2, g * 0.2, b * 0.2, 1);
    draw(2);

    pos = (pos.x, pos.y, pos.z + 2);
    color = black;
    plot(5);
});

var poly = N * Polygons.Square;
var dxs = randBuffer(N);
var dys = randBuffer(N);
var r = randBuffer(N);
var g = randBuffer(N);
var b = randBuffer(N);

Window.ClearColor = white;
Window.OnRender += () => mr(poly, dxs, dys, r, g, b);

Window.CloseOn(Input.Escape);
Window.Open();