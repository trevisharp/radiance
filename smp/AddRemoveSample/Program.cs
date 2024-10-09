using System;
using System.Collections.Generic;

using Radiance.Primitives;
using Radiance.Renders;
using Radiance.Ensemble;

/*
var renders = new RenderCollection(myRenderWith2Params);

var superRender = 
    from i in renders
    let x = 4f * t
    let y = dataList[i]
    where i < 8
    select (x, y); // Cria um render 

Window.OnRender += () => superRender(myPoly);

var query2 =
    coll.Select(r => new { r, x = r.ToString()}) // Criar um objeto Wrapper<T> com conversão implicita:
    
var x = func(new { value = 3 });

Wrapper<T> func<T>(T value)
    => new Wrapper<T>(value);

public class Wrapper<T>(T obj) { }
*/

UnionRender triangule = render((dx, dy, sp, r, g, b, factor, cx, cy) =>
{
    var dist = distance((dx, dy), (cx, cy));

    var scapeX = (dx - cx) / dist;
    var scapeY = (dy - cy) / dist;

    dx += 100 * scapeX / sqrt(dist);
    dy += 100 * scapeY / sqrt(dist);

    rotate(sp * t);
    zoom(factor);
    move(dx, dy);
    color = (r, g, b, 1f);
    fill();
}).ToUnion();

List<float[]> data = [];
for (int i = 0; i < 1_000_000; i++)
{
    data.Add([
        Random.Shared.Next(2000),
        Random.Shared.Next(1200),
        Random.Shared.NextSingle(),
        Random.Shared.NextSingle() / 2,
        Random.Shared.NextSingle() / 2,
        Random.Shared.NextSingle() + .6f + .4f
    ]);
}

Vec2 cursor = (0, 0);
Window.OnMouseMove += p => cursor = p;

var hei = MathF.Sqrt(3) / 2;
var poly = Polygons.FromData((1, -hei), (0, hei), (-1, -hei));
dynamic myRender = triangule
    .AddArgumentFactory(i => data[i][0])
    .AddArgumentFactory(i => data[i][1])
    .AddArgumentFactory(i => data[i][2])
    .AddArgumentFactory(i => data[i][3])
    .AddArgumentFactory(i => data[i][4])
    .AddArgumentFactory(i => data[i][5])
    .SetBreaker(i => i < 1_000_000);

var multRender = poly
    .Select(i => new { i = poly, count = 100 })
    .Select(x => x.i);

var multRender2 = 
    from i in poly
    let count = 100
    select i;

Window.OnRender += () => 
{
    myRender(poly, 10, cursor);
};

Window.CloseOn(Input.Escape);
Window.Open();


// Dynkas app
// var myRender = render(im =>
// {
//     rotate(.5f * t);
//     move(1100, 700);
//     zoom(1100, 700, 1 + sin(t) / 5);
//     color = texture(im, x * im.xratio, y * im.yratio);
//     fill();
// });

// var background = render(im =>
// {
//     color = 0.85f * black + 0.15f * texture(im, x * im.xratio, y * im.yratio);
//     fill();
// });

// Window.OnLoad += () =>
// {
//     myRender = myRender(Polygons.Polar((a, i) => 200 + 200 * (i % 2), 0, 0, -0.2f, 10));
//     background = background(Polygons.Screen);
// };

// var dynkas = open("dynkas.jpg");
// Window.OnRender += () => 
// {
//     background(dynkas);
//     myRender(dynkas);
// };

// Window.CloseOn(Input.Escape);
// Window.Open();