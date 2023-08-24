using Radiance;
using static Radiance.RadianceUtils;

var rect = data(
    n, i, i + j,
    n, j, i + j
);

Window.OnRender += r =>
{
    r.Verbose = true;
    float N = 40;
    for (int i = 0; i < N; i++)
    {
        r.FillTriangles(rect
            .transform(v => (v.x * 20 * (N - i), v.y * 20 * (N - i), 0))
            .colorize(i / N, 0, 0)
        );
    }
};

Window.CloseOn(Input.Escape);

Window.Open();