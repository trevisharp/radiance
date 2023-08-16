using Radiance;
using static Radiance.RadianceUtils;

var region = vecs(
    (-1, -1, 0),
    (+1, -1, 0),
    (-1, +1, 0),

    (+1, +1, 0),
    (+1, -1, 0),
    (-1, +1, 0)
);

var border = vecs(
    (-1, -1, 0),
    (+1, -1, 0),
    (+1, +1, 0),
    (-1, +1, 0)
);

Window.OnRender += r =>
{
    var size = 50 + 20 * cos(5 * t);
    var center = (width / 2, height / 2, 0);

    r.Clear(Color.White);
    
    r.FillTriangles(region
        .transform(v => center + size * v)
        .colorize(cos(t) + 1, sin(t) + 1, 0, 1)
    );

    r.Draw(border
        .transform(v => center + size * v)
        .colorize(Color.Black)
    );
};

Window.CloseOn(Input.Escape);

Window.Open();