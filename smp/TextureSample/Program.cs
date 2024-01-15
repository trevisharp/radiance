using Radiance;
using static Radiance.RadianceUtils;

var myRender = render(r =>
{
    var img1 = open("faustao1.png");
    var img2 = open("faustao2.png");

    // x = 300 * x + width / 2;
    // y = 300 * y + height / 2;

    r.Clear(white);
    r.FillTriangles(ellip(0, 0, 1, 1, 46)
        .triangules()
        .transform(v => (300 * v.x + width / 2, 300 * v.y + height / 2, v.z))
        .colorize(v => 
            mix(
                texture(img1, (300 * v.x / width + 0.5f, 300 * v.y / height + 0.5f)),
                texture(img2, (300 * v.x / width + 0.5f, 300 * v.y / height + 0.5f)),
                (sin(t) + 1) / 2
            )
        )
    );
    r.Draw(ellip(0, 0, 1, 1, 46)
        .triangules()
        .transform(v => (300 * v.x + width / 2, 300 * v.y + height / 2, v.z))
        .colorize(red)
    );
});

Window.OnRender += myRender;

Window.CloseOn(Input.Escape);

Window.Open();