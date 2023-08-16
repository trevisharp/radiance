using Radiance;
using Radiance.Data;
using static Radiance.RadianceUtils;

Window.OnRender += r =>
{
    var size = 50 + 20 * cos(5 * t);
    var center = (width / 2, height / 2, 0);

    r.Clear(Color.White);

    Vectors regionA = new() {
        (-1, -1, 0),
        (+1, -1, 0),
        (-1, +1, 0),

        (+1, +1, 0),
        (+1, -1, 0),
        (-1, +1, 0),
    };
    
    Vectors regionB = new() {
        (-1, -1, 0),
        (+1, -1, 0),
        (+1, +1, 0),
        (-1, +1, 0)
    };
    
    r.Fill(
        v => center + size * v,
        () => Color.Blue,
        regionA
    );

    // r.Draw(
    //     v => center + size * v,
    //     () => Color.Red,
    //     regionB
    // );
};

Window.CloseOn(Input.Escape);

Window.Open();