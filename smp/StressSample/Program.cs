using System;
using Radiance;
using static Radiance.Utils;

var sqRen = render((r, g, b, x, y, w, h) =>
{
    verbose = true;

    pos *= (w, h, 1);
    pos += (x, y, 0);

    color = (r, g, b, 1);
    fill();

    color = black;
    draw();
});

// Window.OnFrame += () => 
//     Console.WriteLine(Window.Fps);

Window.OnRender += () =>
{
    for (int j = 0; j < Window.Height; j += 5)
        for (int i = 0; i < Window.Width; i += 5)
            sqRen(Square, 
                j / 250.0, 0, i / 1000.0,
                i, j, 5.0, 5.0
            );
};

Window.CloseOn(Input.Escape);
Window.Open();