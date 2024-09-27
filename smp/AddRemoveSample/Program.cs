using Radiance;
using static Radiance.Utils;

var myRender = render(im =>
{
    kit.Rotate(.5f);
    kit.Move(1100, 700);
    color = texture(im, (x / width, y / height));
    fill();
});

var background = render(im =>
{
    color = mix(black, texture(im, (x / width, y / height)), 0.15);
    fill();
});

Window.OnLoad += () =>
{
    Window.ZBufferEnable = true;
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