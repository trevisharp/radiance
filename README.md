# Duck

##### Not is the most fast.
##### Not is the most beautiful.
##### Not is the most easy. 
##### But is a bit fast, beautiful and easy.

A library based on OpenTK which is based on OpenGL.
Duck has a API similar to System.Drawing.Graphics.

# Drawing simple 2D objects

```cs
using System.Drawing;

using Duck;

Graphics g = null;

Window.OnLoad += delegate
{
    g = Graphics.New(Window.Width, Window.Height);
};

Window.OnUnload += delegate
{
    g.Dispose();
};

Window.OnRender += delegate
{
    var size = 50;
    var i = Vector.i; // x-axis vector
    var j = Vector.j; // y-axis vector

    var center = ((Window.Width - size) / 2, (Window.Height - size) / 2);

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

Window.OnKeyDown += e =>
{
    if (e == Input.Escape)
        Window.Close();
};

Window.Open();
```