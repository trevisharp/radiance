using Radiance;
using static Radiance.Utils;

var myRender = render(im =>
{
    rotate(.5f * t);
    move(1100, 700);
    zoom(1100, 700, 1 + sin(t) / 5);
    color = texture(im, x * im.xratio, y * im.yratio);
    fill();
});

var background = render(im =>
{
    color = 0.85f * black + 0.15f * texture(im, x * im.xratio, y * im.yratio);
    fill();
});

Window.OnLoad += () =>
{
    myRender = myRender(Polygons.Polar((a, i) => 200 + 200 * (i % 2), 0, 0, -0.2f, 10));
    background = background(Polygons.Screen);
};

var dynkas = open("dynkas.jpg");
Window.OnRender += () => 
{
    background(dynkas);
    myRender(dynkas);
};

Window.CloseOn(Input.Escape);
Window.Open();