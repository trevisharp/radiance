using Radiance;
using static Radiance.Utils;

var otherRender = render(() =>
{
    pos = (100 * x, 100 * y, z);
    color = red;
});

var myRender = render(() =>
{
    otherRender();
    otherRender();
    otherRender();
});

Window.CloseOn(Input.Escape);
Window.Open();