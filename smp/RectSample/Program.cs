using Radiance;
using static Radiance.Utils;

var myRender = render((r, g, b, a) =>
{
    color = (r, g, b, a);
    fill();
});

Window.OnLoad += () => 
    myRender = myRender(
        Rect(
            Window.Width / 2,
            Window.Height / 2,
            0, 500, 500
        )
    );

var fillColor = red;
Window.OnKeyDown += (k, m) =>
{
    if (k != Input.Space)
        return;
    
    fillColor = fillColor == red ? blue : red;
};

Window.OnRender += () => myRender(fillColor);

Window.CloseOn(Input.Escape);
Window.Open();