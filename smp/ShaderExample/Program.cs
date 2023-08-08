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
            layout(1, vec4, out var color);

            gl_Position = vec(pos, 1.0);
            outVar(vec4, "vertexColor", color);
        })
        .SetFragmentShader(() =>
        {
            inVar(vec4, "vertexColor", out var vertexColor);

            gl_FragColor = vertexColor;
        });
};

Window.OnRender += delegate
{
    var red = Color.Red;
    var green = Color.Green;
    var w = Window.Width;
    var h = Window.Height;

    g.FillPolygon(
        (0, 0, 0, red),
        (w, 0, 0, green),
        (w, h, 0, red),
        (0, h, 0, green)
    );
};

Window.CloseOn(Input.Escape);

Window.Open();