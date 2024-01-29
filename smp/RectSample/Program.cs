using Radiance;
using static Radiance.Utils;

var rect = Empty;

Window.OnLoad += () => 
    rect = Rect(
        Window.Width / 2,
        Window.Height / 2,
        0, 500, 500
    );

Window.OnRender += () =>
    Kit.Fill(rect, red);

Window.CloseOn(Input.Escape);
Window.Open();