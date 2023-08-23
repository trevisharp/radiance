using Radiance;
using static Radiance.RadianceUtils;

var end = i + j; 

var region = data(
    n, i, end,
    n, j, end
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
    region.HasChanged();
};

Window.CloseOn(Input.Escape);

Window.Open();