using Radiance;
using Radiance.Renders;
using static Radiance.Utils;

var myRender = render(dz =>
{
    pos += (width / 2, height / 2, dz);
    color = white;
    fill();
});
var rect = Data(
    (0, 0, 0), (100, 0, 0),
    (100, 100, 0), (0, 100, 0)
);

float z = 0;
Window.OnKeyDown += (inp, mod) => z++;
Window.OnRender += () => myRender(rect, z);

Window.CloseOn(Input.Escape);
Window.Open();