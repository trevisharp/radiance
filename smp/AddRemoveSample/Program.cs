using System;
using Radiance;
using static Radiance.Utils;

float cx = 0f, cy = 0f;
Window.OnMouseMove += p => (cx, cy) = p;

float sx = 0f, sy = 0;
float vx = 0f, vy = 0;
float speed = 0;
const float acceleartion = 10f;
const float friction = .5f;

var myRender = render((cx, cy, speed) =>
{
    pos *= 90 + 10 * sin(5 * t);
    pos += (cx, cy, 0);
    color = mix(blue, red, (sin(5 * t) + 1) / 2);
    fill();
});

Window.OnFrame += () =>
{
    var dx = cx - sx;
    var dy = cy - sy;

    vx += acceleartion * dx * dt;
    vy += acceleartion * dy * dt;
    speed = MathF.Sqrt(vx * vx + vy * vy);

    vx *= MathF.Pow(friction, dt);
    vy *= MathF.Pow(friction, dt);

    sx += vx * dt;
    sy += vy * dt;
};

Window.OnRender += () => myRender(Circle, sx, sy, speed);

Window.CloseOn(Input.Escape);
Window.Open();