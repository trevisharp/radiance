using System;
using DuckGL;
using static DuckGL.Shaders;

Graphics g = null;
DateTime start = DateTime.Now;

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
            uniform(single, out var time);

            var red = vec(1.0, 0.0, 0.0);
            var blue = vec(0.0, 0.0, 1.0);

            var timeData = sin(time);
            var interpolation = smoothstep(-1, 1, timeData);

            gl_FragColor = vec(mix(red, blue, interpolation), 1.0);
        });
};

Window.OnRender += delegate
{
    var time = (DateTime.Now - start).TotalSeconds;
    
    var w = Window.Width;
    var h = Window.Height;

    g.FillPolygon(
        time,
        (0, 0, 0),
        (w, 0, 0),
        (w, h, 0),
        (0, h, 0)
    );
};

Window.CloseOn(Input.Escape);

Window.Open();