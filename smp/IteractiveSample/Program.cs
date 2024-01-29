using Radiance;
using static Radiance.Utils;

float px = 0;
float py = 0;

var horMov = 0f;
var verMov = 0f;

var maxSpeed = 500;

Window.OnLoad += delegate
{
    px = Window.Width / 2 - 25;
    py = Window.Height / 2 - 25;
};

Window.OnFrame += delegate
{
    if (horMov > maxSpeed)
        horMov = maxSpeed;
    else if (horMov < -maxSpeed)
        horMov = -maxSpeed;

    if (verMov > maxSpeed)
        verMov = maxSpeed;
    else if (verMov < -maxSpeed)
        verMov = -maxSpeed;
    
    px += horMov * dt;
    py += verMov * dt;
    
    if (horMov > 0)
        horMov -= maxSpeed * dt;
    else if (horMov < 0)
        horMov += maxSpeed * dt;

    if (verMov > 0)
        verMov -= maxSpeed * dt;
    else if (verMov < 0)
        verMov += maxSpeed * dt;
};

var simple = render((px, py) =>
{
    verbose = true;
    pos *= 50;
    pos += (px, py, 0);
    color = red;
    draw();
});
var rect = Rect(1, 1);

Window.OnRender += () => simple(rect, px, py);

Window.OnKeyDown += (input, modifier) =>
{
    if (input == Input.D)
        horMov = maxSpeed;
    
    if (input == Input.A)
        horMov = -maxSpeed;
    
    if (input == Input.W)
        verMov = maxSpeed;

    if (input == Input.S)
        verMov = -maxSpeed;
};

Window.CloseOn(Input.Escape);

Window.Open();