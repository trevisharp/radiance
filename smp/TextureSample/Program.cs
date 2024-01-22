using Radiance;
using static Radiance.Utils;

var myRender = render(() =>
{
    var img1 = open("faustao1.png");
    var img2 = open("faustao2.png");

    clear(white);
    pos = (500 * pos.x + width / 2, 500 * pos.y + height / 2, 0);
    // var point = (300 * pos.x / width + 0.5f, 300 * pos.y / height + 0.5f);
    // color = mix(texture(img1, point), texture(img2, point), (sin(t) + 1) / 2);
    color = black;
    fill();
});

Window.OnRender += () => myRender(Circle);

Window.CloseOn(Input.Escape);

Window.Open();