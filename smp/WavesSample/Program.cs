﻿using Radiance;
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

const int N = 16;
const int R = 4;
var mr = render((dx, dy, dt, r, g, b) => {
    zoom(40);
    move(
        width * mix(0.15, 0.85, dx),
        height * mix(0.15, 0.85, dy)
    );
    move(0, 10 * sin(4 * t + 5 * dt));
    color = (r, g, b, 1);
    fill();

    move(0, 0, 1);
    color = (r * 0.2, g * 0.2, b * 0.2, 1);
    draw(3);
});

var poly = N * Polygons.Square;
var dxs = randBuffer(N);
var dys = randBuffer(N);
var dts = randBuffer(N);
var r = R * randBuffer(N / R);
var g = R * randBuffer(N / R);
var b = R * randBuffer(N / R);

Window.ClearColor = white;
Window.OnRender += () => mr(poly, dxs, dys, dts, r, g, b);

Window.CloseOn(Input.Escape);
Window.Open();