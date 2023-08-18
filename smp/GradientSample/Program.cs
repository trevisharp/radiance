using Radiance;
using static Radiance.RadianceUtils;

var region = data(
    n | Color.Red,
    i | Color.Blue,
    i + j | Color.Red,

    n | Color.Red,
    j | Color.Blue,
    i + j | Color.Red
);

Window.OnRender += r =>
{   
    r.FillTriangles(region
        .transform((v, u) => ((width * v.x, height * v.y, v.z), u))
        .colorize(u => (u, 1.0))
    );
};

Window.CloseOn(Input.Escape);

Window.Open();