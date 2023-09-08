using Radiance;
using static Radiance.RadianceUtils;

var r1 = rect(50, 50, 200, 100);
var r2 = ellip(250, 150, 200, 50);
var r3 = ellip(350, 250, 50, 50, 6);

var r4 = 
    rect(250, 350, 200, 50)
    .triangules();

Window.OnRender += r =>
{
    r.Draw(r1);
    r.Draw(r2);
    r.Draw(r3);

    r.FillTriangles(r4);
};

Window.CloseOn(Input.Escape);

Window.Open();