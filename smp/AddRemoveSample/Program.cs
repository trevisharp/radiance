using System;
using Radiance.Primitives;

// var myRender = render((dx, dy, sp, r, g, b, factor, cx, cy) =>
// {
//     var dist = distance((dx, dy), (cx, cy));
//     var scapeX = (dx - cx) / dist;
//     var scapeY = (dy - cy) / dist;

//     var len = 350 + 50 * sin(2 * t);
//     dx += len * scapeX / sqrt(dist);
//     dy += len * scapeY / sqrt(dist);

//     rotate(sp * t);
//     zoom(factor);
//     move(dx, dy);
//     color = (r, g, b, 1f);
//     fill();
// });

// myRender = myRender(
//     2 * Polygons.Triangule,
//     forVertex(i => Random.Shared.Next(2000)),
//     forVertex(i => Random.Shared.Next(1200)),
//     forVertex(i => Random.Shared.NextSingle()),
//     forVertex(i => Random.Shared.NextSingle()),
//     forVertex(i => Random.Shared.NextSingle()),
//     forVertex(i => Random.Shared.NextSingle())
// );

// Vec2 cursor = (0, 0);
// Window.OnMouseMove += p => cursor = p;
// Window.OnRender += () => myRender(10, cursor);
// Window.CloseOn(Input.Escape);
// Window.Open();

// Dynkas app
var myRender = render(im =>
{
    rotate(.5f * t);
    move(1100, 700);
    zoom(1100, 700, 1 + sin(t) / 5);
    color = texture(im, x * im.xratio, y * im.yratio);
    fill();
});

var background = render(im =>
{
    color = 0.85f * black + 0.15f * texture(im, x * im.xratio, y * im.yratio);
    fill();
});

Window.OnLoad += () =>
{
    myRender = myRender(Polygons.Polar((a, i) => 200 + 200 * (i % 2), 0, 0, -0.2f, 10));
    background = background(Polygons.Screen);
};

var dynkas = open("dynkas.jpg");
Window.OnRender += () => 
{
    background(dynkas);
    myRender(dynkas);
};

Window.CloseOn(Input.Escape);
Window.Open();