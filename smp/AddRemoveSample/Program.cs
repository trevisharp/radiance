using Radiance;
using static Radiance.Utils;

var myFill = render((dx, dy) =>
{
    pos = (50 * x + dx, 50 * y + dy, z);
    color = red;
    fill();

    color = black;
    pos = (x, y, z - 1);
    draw();
});

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

Window.OnFrame += () =>
{
    clear(white);
    myFill(Square, 100, 100);
    myFill(Circle, 650, 650);

};

Window.CloseOn(Input.Escape);
Window.Open();