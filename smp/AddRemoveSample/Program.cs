using Radiance;
using static Radiance.Utils;

var myRender = render(() =>
{
    verbose = true;
    clear(black);

    pos += center;
    
    var scale = (x - 50) / 50;
    color = (scale, 0, 1, scale);
    fill();
});

var rect = Rect(500, 500);

Window.OnRender += () => myRender(rect);

Window.CloseOn(Input.Escape);

Window.Open();