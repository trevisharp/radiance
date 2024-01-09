using Radiance;
using static Radiance.RadianceUtils;

var myRender = render(r =>
{
    var img = open("pain.png");
    var mod = 400 * sin(t / 5);
    r.Clear(white);
    r.Verbose = true;
    r.FillTriangles(circle
        .triangules()
        .transform(v => (mod * v.x + width / 2, mod * v.y + height / 2, v.z))
        .colorize(v => texture(img, (mod * v.x / width + 0.5f, mod * v.y / height + 0.5f)))
    );
});

Window.OnRender += myRender;

Window.CloseOn(Input.Escape);

Window.Open();