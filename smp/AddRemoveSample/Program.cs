using System;
using System.Collections.Generic;
using System.Linq;
using Radiance.Renders;
using Radiance.Shaders.Objects;

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

var triangule = new UnionRender((
    FloatShaderObject dx, 
    FloatShaderObject dy, 
    FloatShaderObject sp, 
    FloatShaderObject r, 
    FloatShaderObject g, 
    FloatShaderObject b, 
    FloatShaderObject factor,
    FloatShaderObject cx,
    FloatShaderObject cy) =>
{
    var dist = distance((dx, dy), (cx, cy));

    var scapeX = (dx - cx) / dist;
    var scapeY = (dy - cy) / dist;

    dx += 10 * scapeX / dist;
    dy += 10 * scapeY / dist;

    rotate(sp * t);
    zoom(factor);
    move(dx, dy);
    color = (r, g, b, 1f);
    fill();
});

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


float cx = 0f, cy = 0f;
Window.OnMouseMove += p => (cx, cy) = p;

var poly = Polygons.FromData((1, 0), (0, MathF.Sqrt(3)), (-1, 0));
dynamic myRender = triangule
    .AddArgumentFactory(i => data[i][0])
    .AddArgumentFactory(i => data[i][1])
    .AddArgumentFactory(i => data[i][2])
    .AddArgumentFactory(i => data[i][3])
    .AddArgumentFactory(i => data[i][4])
    .AddArgumentFactory(i => data[i][5])
    .SetBreaker(i => i < 1_000_000);

Console.Clear();
Queue<float> fspQueue = new();
Window.OnFrame += () => 
{
    Console.CursorLeft = 0;
    Console.CursorTop = 0;
    fspQueue.Enqueue(Window.Fps);
    if (fspQueue.Count > 10)
        fspQueue.Dequeue();
    Console.WriteLine(fspQueue.Average());
};

Window.OnRender += () => 
{
    myRender(poly, 10, cx, cy);
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