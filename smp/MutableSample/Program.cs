using Radiance;
using static Radiance.RadianceUtils;

var w = i;
var h = j;
var end = i + j;

var region = data(
    n, w, end,
    n, h, end
);

Window.OnRender += r =>
{
    r.Clear(black);

    r.FillTriangles(region
        .colorize(red)
    );
};

Window.OnFrame += () =>
{
    end.x++;
    end.y++;
    w.x++;
    h.y++;
    region.HasChanged();
};

Window.CloseOn(Input.Escape);

Window.Open();