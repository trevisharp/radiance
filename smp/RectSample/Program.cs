using Radiance;
using Radiance.Data;
using static Radiance.RadianceUtils;

Window.OnRender += r =>
{
    var size = 50 + 20 * cos(t);
    var center = (width / 2, height / 2, 0);

    r.Clear(Color.White);
    
    r.Fill(
        v => center + size * v,
        () => Color.Blue,
        (-1, -1, 0),
        (+1, -1, 0),
        (+1, +1, 0),
        (-1, +1, 0)
    );

    // // drawing border of square
    // g.Draw(
    //     Color.Black,
    //     topLeftPt,
    //     topRightPt,
    //     botRightPt,
    //     botLeftPt
    // );
};

Window.CloseOn(Input.Escape);

Window.Open();