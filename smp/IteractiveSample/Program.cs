using Radiance;
using Radiance.Types;
using static Radiance.RadianceUtils;

gfloat x = 0;
gfloat y = 0;

var horMov = 0f;
var verMov = 0f;

var maxSpeed = 500;

var region = data(n, i, i + j, j);

Window.OnLoad += delegate
{
    x = Window.Width / 2 - 25;
    y = Window.Height / 2 - 25;
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
    
    x += horMov * dt;
    y += verMov * dt;
    
    if (horMov > 0)
        horMov -= maxSpeed * dt;
    else if (horMov < 0)
        horMov += maxSpeed * dt;

    if (verMov > 0)
        verMov -= maxSpeed * dt;
    else if (verMov < 0)
        verMov += maxSpeed * dt;
};

Window.OnRender += r =>
{
    r.Draw(region
        .transform(v => (v.x * 50 + x, v.y * 50 + y, v.z))
    );
};

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