using Radiance;
using static Radiance.Utils;

var render1 = render(r =>
{
    pos += (50, 50, 0);
    var scale = (x - 50) / 50;
    color = (scale, 0, 1, 1);
    draw();
});

var render2 = render(r =>
{
    pos += (100, 100, 0);
    var scale = (y - 100) / 50;
    color = (0, scale, 1, 1);
    draw();
});

var rect = Rect(50, 50);
Window.OnRender += render1(rect);
Window.OnRender += render2(rect);

Window.OnKeyDown += (key, mod) =>
{
    switch (key)
    {
        case Input.A:
            render1.Toggle();
            break;

        case Input.S:
            render2.Toggle();
            break;
        
        case Input.D:
            render1.SoftHide();
            render2.SoftHide();
            break;
    }
};

Window.CloseOn(Input.Escape);

Window.Open();