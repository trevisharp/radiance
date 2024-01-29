using Radiance;
using Radiance.Renders;
using static Radiance.Utils;

var oncursor = render((cx, cy) =>
{
    verbose = true;
    pos *= (width, height, 1);
    var cursor = (cx, cy, 0);
    var d = distance(pos, cursor);
    var s = (5.0 + 0.01 * sin(10 * t)) / d;
    color = (s, s, s, 1);
    fill();
});

float x = 0f;
float y = 0f;
Window.OnMouseMove += p => (x, y) = p;

var rect = Rect(1, 1);
Window.OnRender += () => oncursor(rect, x, y);

Window.CursorVisible = false;

Window.CloseOn(Input.Escape);

Window.Open();