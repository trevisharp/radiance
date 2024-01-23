using Radiance;
using static Radiance.Utils;

OpenGLManager.Verbose = true;
var myRender = render((img1, img2) =>
{
    clear(white);

    pos *= 500;
    pos += center;
    
    var point = (1, 1);
    color = mix(texture(img1, point), texture(img2, point), (sin(t) + 1) / 2);
    fill();
});

Window.OnRender += () =>
    myRender(Circle, open("faustao1.png"), open("faustao2.png"));

Window.CloseOn(Input.Escape);

Window.Open();