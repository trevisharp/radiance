using Radiance;
using static Radiance.RadianceUtils;

var obj1 = render(r =>
{
    r.Draw(
        rect(50, 50, 50, 50)
        .colorize(v => {
            var scale = (v.x - 50) / 50;
            return (scale, 0, 1, 1);
        })
    );
});

var obj2 = render(r =>
{
    r.Draw(
        rect(100, 100, 50, 50)
        .colorize(v => {
            var scale = (v.y - 100) / 50;
            return (0, scale, 1, 1);
        })
    );
});

Window.OnRender += obj1; // Or obj1.StartRender();
Window.OnRender += obj2; // Or obj2.StartRender();

Window.OnKeyDown += (key, mod) =>
{
    switch (key)
    {
        case Input.A:
            obj1.ToggleRender();
            break;

        case Input.S:
            obj2.ToggleRender();
            break;
    }
};

Window.CloseOn(Input.Escape);

Window.Open();