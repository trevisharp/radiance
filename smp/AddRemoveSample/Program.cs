using Radiance;
using static Radiance.Utils;

var render1 = render(() =>
{
    pos += (width / 2, height / 2, 0);
    var scale = (x - 50) / 50;
    color = (scale, 0, 1, scale);
    fill();
});

var render2 = render(() =>
{
    pos += (width / 2, height / 2, 0);
    var scale = (y - 100) / 50;
    color = (0, scale, 1, scale);
    fill();
});

var rect = Rect(500, 500);
var drawRect1 = render1(rect);
var drawRect2 = render2(rect);

Window.OnRender += drawRect1;
Window.OnRender += drawRect2;

Window.OnKeyDown += (key, mod) =>
{
    switch (key)
    {
        case Input.A:
            Window.OnRender -= drawRect1;
            break;

        case Input.S:
            Window.OnRender -= drawRect2;
            break;

        case Input.D:
            Window.OnRender += drawRect1;
            Window.OnRender += drawRect2;
            break;
    }
};

Window.CloseOn(Input.Escape);

Window.Open();