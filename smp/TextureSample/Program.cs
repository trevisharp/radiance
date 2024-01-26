using Radiance;
using Radiance.Renders;
using static Radiance.Utils;

RenderContext.Verbose = true;
var myRender = render((img1, img2) =>
{
    clear(white);

    pos *= 400;
    pos += center;
    
    var point = (x / width, y / height);
    var text1 = texture(img1, point);
    var text2 = texture(img2, point);
    color = mix(text1, text2, (sin(t) + 1) / 2);
    fill();
});

var f1 = open("faustao1.png");
var f2 = open("faustao2.png");
var f3 = open("faustao3.jpg");
var faustao = myRender(Circle, f1);

var img = f2;

Window.OnRender += () => faustao(img);

Window.OnKeyDown += (key, mod) =>
{
    if (key == Input.Space)
        img = img == f2 ? f3 : f2;
};

Window.CloseOn(Input.Escape);
Window.Open();