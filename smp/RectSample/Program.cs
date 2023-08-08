using Radiance;
using static Radiance.Shaders;

Graphics g = null;

Window.OnLoad += delegate
{
    g = Window
    .CreateGraphics()
    .SetVertexShader(() =>
    {
        layout(0, vec3, out var pos);
        gl_Position = vec(pos, 1.0);
    })
    .SetFragmentShader(() =>
    {
        uniform(vec4, out var color);
        gl_FragColor = color;
    });
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