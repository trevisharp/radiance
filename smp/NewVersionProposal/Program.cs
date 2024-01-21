using Radiance;
using static Radiance.Utils;

Window.OnLoad += delegate
{
    OpenGLManager.Verbose = true;
    var myRender = render(() =>
    {
        clear(white);

        pos += (width / 2, height / 2, 0);
        pos += (5 * sin(t), 5 * cos(t), 0);

        color = black;
        draw();
    });

    var data = Rect(50, 50);

    Window.OnRender += () => myRender(data);
};

Window.CloseOn(Input.Escape);
Window.Open();