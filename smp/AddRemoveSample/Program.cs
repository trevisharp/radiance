using Radiance;
using static Radiance.Utils;

var bounce = render((radius, speed) =>
    pos += (radius * cos(speed * t), radius * sin(speed * t), 0)
);

var colorify = render(() =>
{
    color = (rand((x, y)), rand((x, y)), rand((x, y)), 1f);
});

var bounceFill = render((speed) =>
{
    verbose = true;
    f(speed);
    pos *= (100, 100, 0);
    pos += (width / 2, height / 2, 0);
    bounce(50, speed);
    colorify();
    fill();
});

var star = render((im, cx, cy, size) =>
{
    var d = distance((x, y), (cx, cy));
    var s = size * (1 + 0.05 * sin(10 * t)) / d;
    color = mix(black, texture(im, (x / width, y / height)), min(s, 1));
    fill();
});
Window.OnLoad += () => star = star(Screen, open("dynkas.jpg"));

float cx = 0, cy = 0, size = 1f;
Window.OnMouseMove += p => (cx, cy) = p;
Window.OnMouseWhell += whell => size = float.Max(size + whell, 1f);

Window.OnRender += () => 
{
    // star(cx, cy, size);
    bounceFill(Square, 2);
};

Window.CursorVisible = false;
Window.CloseOn(Input.Escape);
Window.Open();