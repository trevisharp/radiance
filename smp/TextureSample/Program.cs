using Radiance;
using static Radiance.RadianceUtils;

var myRender = render(r =>
{
    var img = open("pain.png");
    var mod = 400 * sin(t / 5);
    r.FillTriangles(ellip(0, 0, 1)
        .triangules()
        .transform(v => (mod * v.x + width / 2, mod * v.y + height / 2, v.z))
        .colorize(v => texture(img, (v.x, v.y)))
    );
    r.Draw(ellip(0, 0, 1)
        .transform(v => (mod * v.x + width / 2, mod * v.y + height / 2, v.z))
        .colorize(v => white)
    );
});

Window.OnRender += myRender;

Window.CloseOn(Input.Escape);

Window.Open();