using System;
using Radiance;
using static Radiance.Utils;

var myRender = render(() =>
{
    verbose = true;
    clear(black);

    pos += center;
    
    var scale = x / width;
    color = (scale, 0, 1, 1);
    fill();
}).Curry(Rect(500, 500));

bool isVisible = true;
Window.OnRender += myRender;

Window.OnKeyDown += (key, mod) =>
{
    if (key != Input.Space)
        return;
    
    if (isVisible)
        Window.OnRender -= myRender;
    else Window.OnRender += myRender;

    isVisible = !isVisible;
};

Window.CloseOn(Input.Escape);
Window.Open();