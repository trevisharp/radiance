using Radiance;
using static Radiance.RadianceUtils;

var draw = CreateRender(op =>
{
    var size = 50 + 20 * cos(t);
    var center = (width / 2, height / 2, 0);

    // using vetorial algebra to build a centralized square
    var topLeftPt  = center + size * (-i -j);
    var topRightPt = center + size * (+i -j);
    var botRightPt = center + size * (+i +j);
    var botLeftPt  = center + size * (-i +j);

    // clear scream
    // g.Clear(Color.White);
    
    // // filling square
    // g.Fill(
    //     Color.Blue,
    //     topLeftPt,
    //     topRightPt,
    //     botRightPt,
    //     botLeftPt
    // );

    // // drawing border of square
    // g.Draw(
    //     Color.Black,
    //     topLeftPt,
    //     topRightPt,
    //     botRightPt,
    //     botLeftPt
    // );
});

Window.OnRender += delegate
{
    draw();
};

Window.CloseOn(Input.Escape);

Window.Open();