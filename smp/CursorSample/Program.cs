using Radiance;
using static Radiance.RadianceUtils;

var region = data(
    n, i, i + j,
    n, j, i + j
);

var x = globalSingle;
var y = globalSingle;

Window.OnRender += r => 
    r.FillTriangles(region
        .transform(v => (width * v.x, height * v.y, 0))
        .colorize(x / width, y / height, 0f)
    );

Window.OnMouseMove += p => (x, y) = p;

Window.CloseOn(Input.Escape);

Window.Open();