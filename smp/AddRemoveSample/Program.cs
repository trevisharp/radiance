using Radiance;
using static Radiance.Utils;

var drawRect = render((z, delta, r, g) =>
{
    kit.Centralize();
    pos = (pos.x + delta, pos.y + delta, z);
    color = (r, g, 0, 1);
    fill();
});
drawRect = drawRect(Rect(0, 0, 0, 100, 100));


float cx = 0, cy = 0, size = 1f;
Window.OnMouseMove += p => (cx, cy) = p;
Window.OnMouseWhell += whell => size = float.Max(size + whell, 1f);

Window.OnKeyDown += (key, mod) =>
{
    if (key == Input.Space)
        Window.ZBufferEnable = !Window.ZBufferEnable;
};

Window.OnRender += () => 
{
    drawRect(-.5, 20, 0, 1);
    drawRect(0, 40, 1, 1);
    drawRect(.5, 0, 1, 0);
};

Window.CursorVisible = false;
Window.CloseOn(Input.Escape);
Window.Open();