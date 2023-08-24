using Radiance;
using static Radiance.RadianceUtils;

var x = 0f;
var y = 0f;

var cursor = i + j;

var region = data(
    n | black, i | black, cursor | white,
    n | black, j | black, cursor | white,

    2 * j | black, j | black, cursor | white,
    2 * j | black, 2 * j + i | black, cursor | white,

    2 * i | black, 2 * i + j | black, cursor | white,
    2 * i | black, i | black, cursor | white,

    2 * i + 2 * j | black, 2 * i + j | black, cursor | white,
    2 * i + 2 * j | black, 2 * j + i | black, cursor | white
);

Window.OnRender += r =>
{
    r.FillTriangles(region
        .transform((v, c) => (width * v.x / 2, height * v.y / 2, 0))
        .colorize((v, c) => c)
    );
};

Window.OnFrame += delegate
{
    cursor.x = 2 * x / Window.Width;
    cursor.y = 2 * y / Window.Height;
    region.HasChanged();
};

Window.OnMouseMove += p => (x, y) = p;

Window.CloseOn(Input.Escape);

Window.Open();