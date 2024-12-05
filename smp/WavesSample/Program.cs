using Radiance;
using static Radiance.Utils;

using System;

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
const int N = 32;

var mr = render((dx, dy, rdx, rdy, r, g, b) => {
    color = (r, g, b, 1);
    move(dx, dy);

    animation(b => {

        const float dt = 0.4f;
        b.Wait(2 * dt);

        var sum = 1.5 * (dx + dy);
        var dif = 1.5 * (dx - dy);

        b.Step(dt, a => move(a * dx / 2, a * dy / 2));
        b.Step(dt, a => move(-a * dif, -a * sum));
        b.Step(dt, a => move(-a * dy / 2, a * dx / 2));

        b.Wait(2 * dt);

        b.Step(dt, a => move(a * dy / 2, -a * dx / 2));
        b.Step(dt, a => move(-a * sum, a * dif));
        b.Step(dt, a => move(a * dx / 2, a * dy / 2));

        b.Wait(2 * dt);
        
        b.Step(dt, a => move(-a * dx / 2, -a * dy / 2));
        b.Step(dt, a => move(a * dif, a * sum));
        b.Step(dt, a => move(a * dy / 2, -a * dx / 2));

        b.Wait(2 * dt);

        b.Step(dt, a => move(-a * dy / 2, a * dx / 2));
        b.Step(dt, a => move(a * sum, -a * dif));
        b.Step(dt, a => move(-a * dx / 2, -a * dy / 2));

        b.Loop();
        
    });

    move(width * rdx, height * rdy);
    fill();

    color = black;
    move(k);
    draw(2f);
});

var poly = 4 * N * Polygons.Rect(40, 40);
var dxs = buffer(4 * N, i => i % 4 is 0 or 1 ? 20 : -20);
var dys = buffer(4 * N, i => i % 4 is 0 or 3 ? 20 : -20);
var rdx = 4 * randBuffer(N);
var rdy = 4 * randBuffer(N);
var r = randBuffer(4 * N);
var g = randBuffer(4 * N);
var b = randBuffer(4 * N);

Window.ClearColor = white;
Window.OnRender += () => mr(poly, dxs, dys, rdx, rdy, r, g, b);

Window.CloseOn(Input.Escape);
Window.Open();