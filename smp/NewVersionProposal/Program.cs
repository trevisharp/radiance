using Radiance;
using static Radiance.Utils;

var myRender = render((cr, cg, cb, amp) =>
{
    x += width / 2;
    y += height / 2;

    x += amp * sin(t);  
    y += amp * cos(t);

    (r, g, b, a) = black;
    draw();

    (r, g, b, a) = (cr, cg, cb, 1);
    fill();
});

var data = Rect(50, 50);

Window.OnRender += () => myRender(data, red, 5);

Window.CloseOn(Input.Escape);
Window.Open();