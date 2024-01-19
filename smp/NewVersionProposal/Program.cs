using Radiance;
using static Radiance.Utils;
using static Radiance.RadianceUtils;

var myRender = render((cr, cg, cb, amp) =>
{
    x += width / 2;
    y += height / 2;

    x += amp * sin(t);  
    y += amp * cos(t);

    r = 0;
    g = 0;
    b = 0;

    draw();

    r = cr;
    g = cg;
    b = cb;

    fill();
});

var data = rect(50, 50);

Window.OnRender += () => myRender(data, red, 5);

Window.CloseOn(Input.Escape);
Window.Open();