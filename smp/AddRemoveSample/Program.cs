using Radiance;
using static Radiance.Utils;

var otherRender = render(() =>
{
    
});

var myRender = render(() =>
{
    otherRender();
    otherRender();
    otherRender();
});

Window.CloseOn(Input.Escape);
Window.Open();