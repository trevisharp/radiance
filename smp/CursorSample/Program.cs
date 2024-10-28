using Radiance;
using static Radiance.Utils;

var myRender = render((size, dx, dy) => {
    zoom(size);
    move(dx + 10 * t, dy + 10 * t);
    // mix recive two values and a coeficient between 0 and 1
    // and chooses a mix of the values using this coeficient
    // 0 = red, 1 = blue, 0.5 = (red + blue) / 2
    // sin is a trigonometric function
    color = mix(red, blue, sin(t));
    fill();
});

Window.OnRender += () => myRender(
    Polygons.Square,
    100, 200, 200
);

Window.CloseOn(Input.Escape);
Window.Open();