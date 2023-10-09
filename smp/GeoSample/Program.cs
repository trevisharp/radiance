using Radiance;
using static Radiance.RadianceUtils;

var r1 =
    rect(50, 50, 200, 100)
    .triangules();

var r2 = 
    ellip(500, 500, 200, 50)
    .triangules();
    
var r3 =
    ellip(350, 250, 50, 50, 6)
    .triangules();

var r4 = 
    rect(250, 350, 200, 50)
    .triangules();

Window.OnRender += r =>
{
    r.FillTriangles(r1);
    r.FillTriangles(r2);
    r.FillTriangles(r4);
    r.FillTriangles(r3);
};

Window.CloseOn(Input.Escape);

Window.Open();