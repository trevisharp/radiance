using Radiance;
using static Radiance.Utils;

var rect = Rect(50, 50, 0, 500, 500);
Window.OnRender += () 
    => Kit.SimpleFill(rect, red);

Window.CloseOn(Input.Escape);

Window.Open();