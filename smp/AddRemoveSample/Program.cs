const int N = 10;

var myRender = render((dx, dy, size) =>
{
    verbose = true;
    zoom(size);
    move(width * dx, height * dy);
    color = mix(red, blue, x / width);
    fill();
});

var poly = N * Polygons.Triangule;
var dxs = randBuffer(N);
var dys = randBuffer(N);
var sizes = randBuffer(N);

bool invert = false;
Window.OnKeyDown += (key, mod) =>
{
    if (key == Input.Space)
        invert = !invert;
};

Window.OnRender += () => 
{
    myRender(poly, dxs, dys, 100);

    // myRender(
    //     poly,
    //     invert ? dys : dxs,
    //     invert ? dxs : dys,
    //     sizes
    // );

    // myRender(
    //     Polygons.Square,
    //     400, 400, 100
    // );
};
Window.CloseOn(Input.Escape);
Window.Open();

// @@Dynkas app
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