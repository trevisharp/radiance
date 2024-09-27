using Radiance;
using Radiance.Buffers;
using static Radiance.Utils;

var myRender = render(im =>
{
    kit.Rotate(1f);
    kit.Move(1100, 700);
    color = texture(im, (x / width, y / height));
    fill();
});

var background = render(im =>
{
    color = mix(black, texture(im, (x / width, y / height)), 0.15);
    fill();
});

var star = Polygon.Polar((a, i) => 200 + 200 * (i % 2), 0, 0, 0, 10);
var dynkas = open("dynkas.jpg");
Window.OnRender += () => 
{
    background(Polygon.Screen, dynkas);
    myRender(star, dynkas);
};

Window.CloseOn(Input.Escape);
Window.Open();