using Radiance;
using static Radiance.RadianceUtils;

var rect = data(
    n, i, i + j,
    n, j, i + j
);

Window.OnRender += r =>
{
    r.Verbose = true;
    for (int i = 0; i < 10; i++)
    {
        int j = i;
        r.FillTriangles(rect
            .transform(v => (v.x * 50 * (j + 1), v.y * 50 * (j + 1), 0))
            .colorize(25 * j / 255f, 25 * j / 255f, 25 * j / 255f)
        );
    }
};

Window.CloseOn(Input.Escape);

Window.Open();