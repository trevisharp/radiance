using Radiance;
using static Radiance.Utils;


var otherRender = render((dx, dy) =>
{
    pos = (50 * x + dx, 50 * y + dy, z);
    color = red;
    fill();
});

var myRender = render(() =>
{
    otherRender(poly, 100, 100);
    otherRender(poly, 200, 100);
    otherRender(poly, 150, 150);
});

Window.OnFrame += () =>
{
    myRender(Square);
};

Window.CloseOn(Input.Escape);
Window.Open();