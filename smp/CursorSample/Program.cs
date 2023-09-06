using Radiance;
using Radiance.Types;
using static Radiance.RadianceUtils;

gfloat x = 0f;
gfloat y = 0f;

var screen = data(
    n, i, i + j,
    n, j, i + j
);

Window.OnRender += r =>
{
    r.Verbose = true;
    r.FillTriangles(screen
        .transform(v => (v.x * width, v.y * height, v.z))
        .colorize(v => 
        {
            var point = (v.x * width, v.y * height, v.z);
            var cursor = (x, y, 0);
            var d = distance(point, cursor);
            var s = (5.0 + 0.01 * sin(10 * t)) / d;
            return (s, s, s, 0);
        })
    );
};

Window.OnMouseMove += p => (x, y) = p;

Window.CursorVisible = false;

Window.CloseOn(Input.Escape);

Window.Open();