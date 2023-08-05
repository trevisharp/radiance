# Duck

Not is the most fast.

Not is the most beautiful.

Not is the most easy.

But is a bit fast, beautiful and easy.

A library based on OpenTK which is based on OpenGL.

Duck has a API similar to System.Drawing.Graphics.

# Tutorials and Examples

## Drawing simple 2D objects

```cs
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
```
Result:
![result](./smp/RectSample/result.jpg)

## Program Shaders in C#

# Versions

### Duck v0.2.0 (coming soon)

### Duck v0.1.0

 - ![](https://img.shields.io/badge/new-green) Window Class
    - Open and Close Methods
    - Events
 - ![](https://img.shields.io/badge/new-green) Input Enum
 - ![](https://img.shields.io/badge/new-green) Vector Record
 - ![](https://img.shields.io/badge/new-green) Vertex Record
 - ![](https://img.shields.io/badge/new-green) Graphics Class
    - Clear Method
    - DrawPolygon Method
    - FillPolygon Method