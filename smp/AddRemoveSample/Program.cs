using System;
using System.Collections.Generic;
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

var triangule = render((dx, dy, sp, r, g, b, factor) =>
{
    rotate(sp * t);
    zoom(0, 0, factor);
    move(dx, dy);
    color = (r, g, b, 1f);
    fill();
});

triangule = triangule(Polygons.FromData((1, 0), (0, 2), (-1, 0)));

List<float[]> data = [];
for (int i = 0; i < 20_000; i++)
{
    data.Add([
        Random.Shared.Next(2000),
        Random.Shared.Next(1000),
        Random.Shared.NextSingle(),
        Random.Shared.NextSingle(),
        Random.Shared.NextSingle(),
        Random.Shared.NextSingle()
    ]);
}

Console.Clear();
Window.OnFrame += () => 
{
    Console.CursorLeft = 0;
    Console.CursorTop = 0;
    Console.WriteLine(Window.Fps);
};

Window.OnRender += () => 
{
    foreach (var args in data)
        triangule(args, 100);
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