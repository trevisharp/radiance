using Radiance;
using static Radiance.RadianceUtils;

var region = data(
    n | magenta,
    i | cyan,
    i + j | magenta,

    n | magenta,
    j | cyan,
    i + j | magenta
);

Window.OnRender += r =>
{
    r.Verbose = true;
    
    r.FillTriangles(region
        .transform((v, c) => (width * v.x, height * v.y, v.z))
        .colorize((v, c) => c)
    );
};

Window.CloseOn(Input.Escape);

Window.Open();