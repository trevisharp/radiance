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

var r4 = data(
        (700, 200, 0),
        (710, 210, 0),
        (720, 230, 0),
        (730, 300, 0),
        (740, 220, 0),
        (750, 230, 0),
        (760, 270, 0),
        (770, 200, 0),
        (780, 300, 0),
        (790, 220, 0),
        (800, 250, 0),
        (810, 260, 0),
        (820, 230, 0),
        (830, 280, 0),
        (840, 350, 0),
        (850, 280, 0),
        (860, 300, 0),
        (870, 290, 0),
        (880, 200, 0),
        (890, 400, 0),
        (900, 150, 0)
    );
var r4filled = r4.triangules();

Window.OnRender += r =>
{
    r.FillTriangles(r1);
    r.FillTriangles(r2);
    r.FillTriangles(r3);

    r.FillTriangles(r4filled);
    r.Draw(r4.colorize(red));
};

Window.CloseOn(Input.Escape);

Window.Open();