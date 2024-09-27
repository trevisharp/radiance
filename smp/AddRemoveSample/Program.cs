using Radiance;
using static Radiance.Utils;

var myRender = render(im =>
{
    kit.Rotate(1f);
    kit.Move(1100, 700);
    color = mix(blue, texture(im, (x / width, y / height)), (x + y) / (width + height));
    fill();
});

var star = Polar((a, i) => 200 + 200 * (i % 2), 0, 0, 0, 10);
var dynkas = open("dynkas.jpg");
Window.OnRender += () => myRender(star, dynkas);

Window.CloseOn(Input.Escape);
Window.Open();