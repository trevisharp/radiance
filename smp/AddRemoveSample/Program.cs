const int N = 1_000_000;

var myRender = render((dx, dy, r, g, b) =>
{
    zoom(10);
    rotate(t);
    move(width * dx, height * dy);
    pos = (pos.x, pos.y, 1000 * r);
    color = (r, g, b, 1f);
    fill();
});

var poly = N * Polygons.Triangule;
var dxs = randBuffer(N, 3);
var dys = randBuffer(N, 3);
var rs = randBuffer(N, 3);
var gs = randBuffer(N, 3);
var bs = randBuffer(N, 3);

Window.OnRender += () => 
{
    myRender(
        poly, dxs, dys,
        rs, gs, bs
    );
};
Window.CloseOn(Input.Escape);
Window.Open();