using Radiance;
using static Radiance.RadianceUtils;

var render1 = render(r =>
{
    r.Draw(
        rect(50, 50, 50, 50)
        .colorize(v => {
            var scale = (v.x - 50) / 50;
            return (scale, 0, 1, 1);
        })
    );
});

var render2 = render(r =>
{
    r.Draw(
        rect(100, 100, 50, 50)
        .colorize(v => {
            var scale = (v.y - 100) / 50;
            return (0, scale, 1, 1);
        })
    );
});

Window.OnRender += render1; // Or obj1.Show();
Window.OnRender += render2; // Or obj2.Show();

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