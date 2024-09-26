using System;
using Radiance;
using static Radiance.Utils;

var star = Polar(a => 300 + 80 * MathF.Cos(MathF.PI * 10 * a / MathF.Tau), 0, 0, 0, 10);

var myRender = render(() =>
{
    verbose = true;
    kit.Rotate(1f);
    kit.Centralize();
    color = mix(blue, green, (x + y) / (width + height));
    fill();
});

Window.OnRender += () =>
{
    myRender(star);
};

Window.CloseOn(Input.Escape);
Window.Open();