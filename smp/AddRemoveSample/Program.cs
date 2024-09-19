using Radiance;
using static Radiance.Utils;

var star = render((cx, cy, size) =>
{
    var d = distance((x, y), (cx, cy));
    var s = size * (1 + 0.05 * sin(10 * t)) / d;
    color = (s, s, s, s);
    fill();
});
Window.OnLoad += () => star = star(Screen);

float cx = 0, cy = 0, size = 10f;
Window.OnMouseMove += p => (cx, cy) = p;
Window.OnMouseWhell += whell => size = float.Max(size + whell, 1f);

Window.OnRender += () =>
{
    star(cx, cy, size);
    star(Window.Width - cx, Window.Height - cy, size);
    star(cx, Window.Height - cy, size);
    star(Window.Width - cx, cy, size);
};

Window.CursorVisible = false;
Window.CloseOn(Input.Escape);
Window.Open();