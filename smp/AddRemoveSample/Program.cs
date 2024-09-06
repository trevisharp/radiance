using Radiance;
using static Radiance.Utils;

var myFill = render((dx, dy) =>
{
    pos = (50 * x + dx, 50 * y + dy, z);
    color = red;
    fill();
});

var myLine = render((dx, dy) =>
{
    for (int i = 0; i < 10; i += 2)
        myFill(poly, dx + 50 * i, dy);
});

var myRender = render((dx, dy) =>
{
    for (int i = 0; i < 10; i++)
        myLine(poly, 
            dx + (i % 2 == 0 ? 0 : 50), 
            dy + 50 * i
        );
});

Window.OnFrame += () =>
{
    myRender(Square, 200, 200);
    myRender(Circle, 700, 700);

};

Window.CloseOn(Input.Escape);
Window.Open();