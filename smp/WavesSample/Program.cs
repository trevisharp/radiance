using Radiance;
using static Radiance.Utils;

using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
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
const int N = 8;

var mr = render((dx, dy, rdx, rdy, r, g, b) => {
    color = (r, g, b, 1);
    move(dx, dy);

    const float dt = 0.4f;
    var time = mod(t, 21 * dt);
    
    var anim1 = smoothstep(dt, 2 * dt, time);
    move(anim1 * dx / 2, anim1 * dy / 2);
    
    var anim2 = smoothstep(2 * dt, 3 * dt, time);
    rotate(-Math.PI / 2 * anim2);
    
    var anim3 = smoothstep(3 * dt, 4 * dt, time);
    move(-anim3 * dy / 2, anim3 * dx / 2);
    
    var anim4 = smoothstep(6 * dt, 7 * dt, time);
    move(anim4 * dy / 2, -anim4 * dx / 2);
    
    var anim5 = smoothstep(7 * dt, 8 * dt, time);
    rotate(-Math.PI / 2 * anim5);
    
    var anim6 = smoothstep(8 * dt, 9 * dt, time);
    move(anim6 * dx / 2, anim6 * dy / 2);
    
    var anim7 = smoothstep(11 * dt, 12 * dt, time);
    move(-anim7 * dx / 2, -anim7 * dy / 2);
    
    var anim8 = smoothstep(12 * dt, 13 * dt, time);
    rotate(-Math.PI / 2 * anim8);
    
    var anim9 = smoothstep(13 * dt, 14 * dt, time);
    move(anim9 * dy / 2, -anim9 * dx / 2);
    
    var anim10 = smoothstep(16 * dt, 17 * dt, time);
    move(-anim10 * dy / 2, anim10 * dx / 2);
    
    var anim11 = smoothstep(17 * dt, 18 * dt, time);
    rotate(-Math.PI / 2 * anim11);
    
    var anim12 = smoothstep(18 * dt, 19 * dt, time);
    move(-anim12 * dx / 2, -anim12 * dy / 2);

    move(width * rdx, height * rdy);
    fill();
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