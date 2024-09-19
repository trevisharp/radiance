using Radiance;
using static Radiance.Utils;

var img = open("dynkas.jpg");
var star = render((im, cx, cy, size) =>
{
    var d = distance((x, y), (cx, cy));
    var s = size * (1 + 0.05 * sin(10 * t)) / d;
    color = mix(black, texture(im, (x / width, y / height)), min(s, 1));
    fill();
});
Window.OnLoad += () => star = star(Screen, img);

float cx = 0, cy = 0, size = 10f;
Window.OnMouseMove += p => (cx, cy) = p;
Window.OnMouseWhell += whell => size = float.Max(size + whell, 1f);

int mirrorState = 0;
Window.OnKeyDown += (key, mod) => 
    mirrorState = (key == Input.Space) ? 
    (mirrorState + 1) % 3 : mirrorState;

Window.OnRender += () =>
{
    star(cx, cy, size);
    if (mirrorState > 0)
        star(Window.Width - cx, Window.Height - cy, size);
    if (mirrorState > 1)
    {
        star(cx, Window.Height - cy, size);
        star(Window.Width - cx, cy, size);
    }
};

Window.CursorVisible = false;
Window.CloseOn(Input.Escape);
Window.Open();