using Radiance;
using static Radiance.Utils;
using static Radiance.Window;

var myRender = render((r, g, b, a) =>
{
    color = (r, g, b, a);
    fill();
});

OnLoad += () => 
    myRender = myRender(
        Rect(
            Width / 2, Height / 2,
            0, 500, 500
        )
    );

var fillColor = red;
OnKeyDown += (k, m) =>
{
    if (k != Input.Space)
        return;
    
    fillColor = fillColor == red ? blue : red;
};

OnRender += () => myRender(fillColor);

CloseOn(Input.Escape);
Open();