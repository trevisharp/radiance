﻿using Radiance;
using Radiance.Shaders.Objects;
using static Radiance.Utils;

// using Radiance.Shaders;
// using Radiance.Shaders.Objects;

// var f1 = new FloatShaderObject("f1", ShaderOrigin.VertexShader, []);
// return;

const int N = 91;

var mr = render((dx, dy, rdx, rdy, r, g, b) => {
    color = (r, g, b, 1);
    verbose = true;
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

    move(rdx, rdy);
    fill();

    // color = black;
    // move(k);
    // draw(2f);
});

var poly = 4 * N * Polygons.Rect(40, 40);
var dxs = buffer(4 * N, i => i % 4 is 0 or 1 ? 20 : -20);
var dys = buffer(4 * N, i => i % 4 is 0 or 3 ? 20 : -20);
var rdx = 4 * buffer(N, i => 80 + i % 13 * 140);
var rdy = 4 * buffer(N, i => 80 + i / 13 * 140);
var r = randBuffer(4 * N);
var g = randBuffer(4 * N);
var b = randBuffer(4 * N);

Window.ClearColor = white;
Window.OnRender += () => mr(poly, dxs, dys, rdx, rdy, r, 0, b);

Window.CloseOn(Input.Escape);
Window.Open();