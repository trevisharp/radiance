using System.Drawing;

using DuckGL;

Graphics g = null;

Window.OnLoad += delegate
{
    g = Window.CreateGraphics();
};

Window.OnRender += delegate
{
    var size = 50;
    var i = Vector.i; // x-axis vector
    var j = Vector.j; // y-axis vector

    var center = (Window.Width / 2, Window.Height / 2);

    // using vetorial algebra to build a centralized square
    var topLeftPt  = center + size * (-i -j);
    var topRightPt = center + size * (+i -j);
    var botRightPt = center + size * (+i +j);
    var botLeftPt  = center + size * (-i +j);
    
    // clear scream
    g.Clear(Color.White);
    
    // filling square
    g.FillPolygon(
        Color.Blue,
        topLeftPt,
        topRightPt,
        botRightPt,
        botLeftPt
    );

    // drawing border of square
    g.DrawPolygon(
        Color.Black,
        topLeftPt,
        topRightPt,
        botRightPt,
        botLeftPt
    );
};

Window.CloseOn(Input.Escape);

Window.Open();