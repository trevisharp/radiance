using Radiance;
using static Radiance.Utils;

var myRender = render((amp) =>
{
    clear(white);

    pos += (width / 2, height / 2, 0);
    pos += (amp * sin(10 * t), amp * cos(10 * t), 0);

    color = black;
    draw();
});

var data = Rect(50, 50);
float radius = 5f;

Window.OnKeyDown += (key, mod) =>
{
    if (key == Input.D)
        radius++;
    
    if (key == Input.A)
        radius--;
    
    if (radius > 50)
        radius = 50;
    
    if (radius < 0)
        radius = 0;
};

Window.OnRender += () => myRender(data, radius);

Window.CloseOn(Input.Escape);

Window.Open();