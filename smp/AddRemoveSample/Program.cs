using System;
using Radiance;
using static Radiance.Utils;

float cx = 0f, cy = 0f;
Window.OnMouseMove += p => (cx, cy) = p;

float sx = 0f, sy = 0;
float vx = 0f, vy = 0;
float speed = 0;
const float acceleartion = 1f;
const float friction = .9f;

var myRender = render((cx, cy, speed) =>
{
    pos *= 100;
    pos += (cx, cy, 0);
    color = mix(blue, red, speed / 1000);
    fill();
});

Window.OnFrame += () =>
{
    var dx = cx - sx;
    var dy = cy - sy;

    vx += acceleartion * dx * dt;
    vy += acceleartion * dy * dt;
    speed = MathF.Sqrt(vx * vx + vy * vy);

    sx += vx * dt;
    sy += vy * dt;

    vx *= friction * dt;
    vy *= friction * dt;
};

Window.OnRender += () => myRender(Square, sx, sy, speed);

Window.CloseOn(Input.Escape);
Window.Open();