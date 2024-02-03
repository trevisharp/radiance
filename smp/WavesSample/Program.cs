using Radiance;
using static Radiance.Utils;

var shipRender = render((px) =>
{
    pos *= 5;
    pos += (px, height / 2, 0);
    color = red;
    fill();
});

var waveRender = render((px, py, init) =>
{
    var size = 34 * (t - init);
    pos *= size;
    pos += (px, py, 0);
    color = white;
    draw();
});

float shipSpeed = 4f;
float shipPosition = 0f;
Window.OnLoad += () 
    => shipPosition = 0;

Window.OnRender += () =>
    shipRender(Circle, shipPosition);

var tmFrame = new Clock();
var tmWave = new Clock();
Window.OnFrame += () =>
{
    shipPosition += shipSpeed * tmFrame.Time;
    shipSpeed += 3 * tmFrame.Time;
    tmFrame.Reset();

    if (tmWave.Time < 0.2)
        return;
    tmWave.Reset();

    float px = shipPosition;
    float py = Window.Height / 2;
    float secs = Time;
    Window.OnRender += () =>
        waveRender(Circle, px, py, secs);
};

Window.OnKeyDown += (k, m) =>
{
    if (k == Input.Space)
    {
        tmFrame.ToogleFreeze();
        tmWave.ToogleFreeze();
    }
};

Window.CloseOn(Input.Escape);
Window.Open();