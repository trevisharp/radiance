using Radiance;
using static Radiance.Utils;

var oncursor = render((cx, cy) =>
{
    verbose = true;
    pos += (0.5, 0.5, 0);
    pos *= (width, height, 1);

    var d = distance((x, y), (cx, cy));
    var s = (5.0 + 0.01 * sin(10 * t)) / d;
    color = (s, s, s, 1);
    fill();
});

float cx = 0f;
float cy = 0f;
Window.OnMouseMove += p => (cx, cy) = p;

var rect = Rect(1, 1);
Window.OnRender += () => oncursor(rect, cx, cy);

Window.CursorVisible = false;

Window.CloseOn(Input.Escape);

Window.Open();