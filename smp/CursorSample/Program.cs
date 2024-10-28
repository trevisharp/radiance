using Radiance;
using static Radiance.Utils;

var myRender = render(() => {
    var center = (width / 2, height / 2);
    var pixel = (x, y);
    var dist = distance(pixel, center);
    var invDist = 100 / dist;
    var light = min(invDist, 1);
    color = (light, light, light, 1f);
    fill();
});

// 1.Your polygon is the entire screen now
// 2.We using a currying operation. Now render has the
// first parameter fixed on Polygons.Screen
// this is interesting because create a polygon on OnRender
// can produce wrong behaviours or performance loss
// 3.We make the operation in OnLoad to grant that the Screen
// already be initialized when the polygon is created.
// In other case we can do myRender = myRender(Polygons.Rect(200, 200))
// outside OnLoad function.
Window.OnLoad += () => myRender = myRender(Polygons.Screen);
Window.OnRender += () => myRender();

Window.CloseOn(Input.Escape);
Window.Open();