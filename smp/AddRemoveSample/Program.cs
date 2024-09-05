using Radiance;
using static Radiance.Utils;

var otherRender = render(() =>
{
    pos = (x, 2 * y, z);
    color = red;
    fill();
});

var myRender = render(() =>
{
    otherRender(Square);
    otherRender(Square);
    otherRender(Square);
});

Window.CloseOn(Input.Escape);
Window.Open();