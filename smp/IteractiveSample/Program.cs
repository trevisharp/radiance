using Radiance;
using static Radiance.RadianceUtils;

float x = 0;
float y = 0;

var region = data(n, i, i + j, j);

Window.OnLoad += delegate
{
    x = Window.Width / 2 - 25;
    y = Window.Height / 2 - 25;
};

Window.OnRender += r =>
{
    r.Draw(region
        .transform(v => (v.x * 50 + x, v.y * 50 + y, v.z))
    );
};

Window.OnKeyDown += input =>
{
    if (input == Input.D)
        x += 5;
    
    if (input == Input.A)
        x -= 5;
    
    if (input == Input.W)
        y += 5;

    if (input == Input.S)
        y -= 5;
};

Window.CloseOn(Input.Escape);

Window.Open();