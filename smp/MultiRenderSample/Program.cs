using Radiance;
using Radiance.Renders;
using static Radiance.Utils;

var renderA = render((dx, dy) =>
{
    pos += (dx, dy, 0);
    color = white;
    fill();
});

var renderB = render((dx, dy) =>
{
    pos += (dx, dy, 0);
    color = white;
    fill();
});

var rect = Rect(100, 100);
Window.OnRender += () => 
{
    renderA(rect, 0, 0);
    renderB(rect, 100, 100);
};

Window.CursorVisible = false;

Window.CloseOn(Input.Escape);
Window.Open();