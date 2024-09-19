using Radiance;
using static Radiance.Utils;

var star = render((dx, dy, size) =>
{
    var d = distance((x, y), (dx, dy));
    var s = size * (1 + 0.05 * sin(10 * t)) / d;
    color = (s, s, s, s);
    fill();
});

float cx = 0, cy = 0, size = 10f;
Window.OnMouseMove += p => (cx, cy) = p;
Window.OnMouseWhell += s => size = float.Max(size + s, 1f);

Window.OnRender += () =>
{
    star(Screen, 300, 300, size);
    star(Screen, cx, cy, size);
};

Window.CursorVisible = false;
Window.CloseOn(Input.Escape);
Window.Open();