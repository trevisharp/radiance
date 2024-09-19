using Radiance;
using static Radiance.Utils;

var myFill = render((dx, dy) =>
{
    pos = 150 * pos + (dx, dy, 0);
    color = red;
    fill();

    color = white;
    draw();
});

var star = render((dx, dy, size) =>
{
    var d = distance((x, y), (dx, dy));
    var s = size * (1 + 0.05 * sin(10 * t)) / d;
    color = (s, s, s, 1);
    fill();
});

// var myLine = render((dx, dy) =>
// {
//     repeat(
//         () => 5, 
//         i => myFill(poly, dx + 100 * i, dy)
//     );
// });

// var myRender = render((dx, dy) =>
// {
//     repeat(
//         () => 10,
//         i => myLine(poly, dx, dy + 50 * i)
//     );
// });

float cx = 0, cy = 0, size = 10f;

Window.OnMouseMove += p => (cx, cy) = p;

Window.OnMouseWhell += s => size = float.Max(size + s, 1f);

Window.OnRender += () => star(Screen, cx, cy, size);

Window.CursorVisible = false;

Window.CloseOn(Input.Escape);
Window.Open();