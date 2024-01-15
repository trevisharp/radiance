using Radiance;
using static Radiance.RadianceUtils;

/**
    var region = square
        .triangules()
        .append(magenta, cyan, magenta, magenta, cyan, magenta);
 **/
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
    /**
        x *= widht;
        y *= height;
        fill();
     **/
    r.FillTriangles(region
        .transform((v, c) => (width * v.x, height * v.y, v.z))
        .colorize((v, c) => c)
    );
};

Window.CloseOn(Input.Escape);

Window.Open();