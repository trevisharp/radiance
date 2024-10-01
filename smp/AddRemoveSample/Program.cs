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