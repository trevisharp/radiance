var myRender = render((dx, dy, angle) =>
{
    zoom(40);
    rotate(angle);
    move(dx, dy);
    color = red;
    fill();
});

var poly = Polygons.Triangule;
var dxs = buffer([ 300, 300, 300 ]);
var dys = buffer([ 400, 400, 400 ]);
var ang = buffer([ 5f, 5f, 5f ]);

Window.OnFrame += () => 
{

};

Window.OnRender += () => myRender(poly, dxs, dys, ang);
Window.CloseOn(Input.Escape);
Window.Open();