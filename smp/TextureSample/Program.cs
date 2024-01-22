using Radiance;
using static Radiance.Utils;

OpenGLManager.Verbose = true;
var myRender = render(() =>
{
    var img1 = open("faustao1.png");
    var img2 = open("faustao2.png");

    clear(white);

    pos *= 500;
    pos += center;
    
    var point = (x / width, y / height);
    // color = mix(texture(img1, point), texture(img2, point), (sin(t) + 1) / 2);
    color = black;
    fill();
});

Window.OnRender += () => myRender(Circle);

Window.CloseOn(Input.Escape);

Window.Open();