using Radiance;
using static Radiance.Utils;

var myFill = render((dx, dy) =>
{
    verbose = true;
    pos = 150 * pos + (dx, dy, 0);
    color = red;
    fill();

    color = white;
    draw();
});

var drawSquare = myFill(Square);
drawSquare.Load();

// var myLine = render((dx, dy) =>
// {
//     repeat(
//         () => 5, 
//         i => myFill(poly, dx + 100 * i, dy)
//     );
// });

// var myRender = render((dx, dy) =>
// {
//     repeat(
//         () => 10,
//         i => myLine(poly, dx, dy + 50 * i)
//     );
// });

Window.OnRender += () =>
{
    drawSquare(100, 100);
    // myFill(Circle, 650, 650);
};

Window.CloseOn(Input.Escape);
Window.Open();