using Radiance;
using static Radiance.Utils;

var myRender = render((img1, img2) =>
{
    zoom(400);
    centralize();
    var text1 = texture(img1, x * img1.xratio, y * img1.yratio);
    var text2 = texture(img2, x * img2.xratio, y * img2.yratio);
    color = mix(text1, text2, (sin(t) + 1) / 2);
    fill();
});

var f1 = open("faustao1.png");
var f2 = open("faustao2.png");
var f3 = open("faustao3.jpg");
var faustao = myRender(Polygons.Circle, f1);

var img = f2;

Window.OnRender += () => faustao(img);

Window.OnKeyDown += (key, mod) =>
{
    if (key == Input.Space)
        img = img == f2 ? f3 : f2;
};

Window.CloseOn(Input.Escape);
Window.Open();