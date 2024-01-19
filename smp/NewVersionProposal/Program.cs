using Radiance;
using static Radiance.Utils;
using static Radiance.RadianceUtils;

var myRender = render((cr, cg, cb, amp) =>
{
    x += width / 2;
    y += height / 2;

    x += amp * sin(t);  
    y += amp * cos(t);

    (r, g, b) = black;
    draw();

    (r, g, b) = (cr, cg, cb);
    fill();
});

var data = rect(50, 50);

Window._OnRender += () => myRender(data, red, 5);

Window.CloseOn(Input.Escape);
Window.Open();