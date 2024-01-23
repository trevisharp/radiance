using Radiance;
using static Radiance.Utils;

var myclear = render(() =>
{
    clear(black);
});

var render1 = render(() =>
{
    pos += (width / 2, height / 2, 0);
    var scale = 0.8;//(x - 50) / 50;
    color = (scale, 0, 1, scale);
    fill();
});

var render2 = render(() =>
{
    pos += (width / 2, height / 2, 0);
    var scale = 0.8;//(y - 100) / 50;
    color = (0, scale, 1, scale);
    fill();
});

var rect = Rect(500, 500);
Action drawRect1 = render1.Curry(rect);
Action drawRect2 = render2.Curry(rect);

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