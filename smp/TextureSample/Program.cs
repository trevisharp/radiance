using Radiance;
using static Radiance.Utils;

var myRender = render((img1, img2) =>
{
    clear(white);

    pos *= 400;
    pos += center;
    
    var point = (x / width, y / height);
    color = mix(texture(img1, point), texture(img2, point), (sin(t) + 1) / 2);
    fill();
});

var f1 = open("faustao1.png");
var f2 = open("faustao2.png");
var circleMix = myRender(Circle);

Window.OnRender += circleMix(f1, f2);

Window.CloseOn(Input.Escape);

Window.Open();