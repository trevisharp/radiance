using Radiance;
using static Radiance.Utils;

var shipRender = render((px) =>
{
    pos = 5 * pos + (px, height / 2, 0);
    color = red;
    fill();
});

var waveRender = render((px, py, init) =>
{
    var size = 34 * (t - init);
    pos = size * pos + (px, py, 0);
    color = white;
    draw();
});

float shipSpeed = 4f;
float shipPosition = 0f;
Window.OnRender += () =>
    shipRender(Circle, shipPosition);

var clkFrame = new Clock();
var clkWave = new Clock();
Window.OnFrame += () =>
{
    shipPosition += shipSpeed * clkFrame.Time;
    shipSpeed += 3 * clkFrame.Time;
    clkFrame.Reset();

    if (clkWave.Time < 0.2)
        return;
    clkWave.Reset();

    Window.OnRender += () =>
        waveRender(Circle, shipPosition, Window.Height / 2, Time);
};

Window.OnKeyDown += (k, m) =>
{
    if (k == Input.Space)
    {
        clkFrame.ToogleFreeze();
        clkWave.ToogleFreeze();
    }
};

Window.CloseOn(Input.Escape);
Window.Open();