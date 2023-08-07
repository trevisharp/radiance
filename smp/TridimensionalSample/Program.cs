using DuckGL;
using static DuckGL.Shaders;

Graphics g = null;

Window.OnLoad += () => 
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
            gl_FragColor = vec(1, 1, 1, 1);
        });
};

Window.OnRender += () =>
{
    g.Clear(Color.Black);
    
    g.DrawPolygon(
        (100, 100, 0),
        (100, 200, 0),
        (200, 200, 0),
        (200, 100, 0)
    );

    g.DrawPolygon(
        (200, 200, 0),
        (200, 300, 0),
        (300, 300, 0),
        (300, 200, 0)
    );
};

Window.CloseOn(Input.Escape);

Window.Open();