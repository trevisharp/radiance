using Radiance;
using static Radiance.RadianceUtils;

var region = data(
    (0, 0, 0) | (1, 0, 0),
    (1, 0, 0) | (0, 0, 1),
    (1, 1, 0) | (1, 0, 0),

    (0, 0, 0) | (1, 0, 0),
    (0, 1, 0) | (0, 0, 1),
    (1, 1, 0) | (1, 0, 0)
);

Window.OnRender += r =>
{   
    r.FillTriangles(region
        .transform((v, u) => ((width * v.x, height * v.y, v.z), u))
        .colorize(u => u)
    );
};

Window.CloseOn(Input.Escape);

Window.Open();