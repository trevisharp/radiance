using DuckGL;
using static DuckGL.Shaders;

Graphics g = null;

Window.OnLoad += delegate
{
    g = Window
        .CreateGraphics()
        .SetVertexShader(() =>
        {
            version("330 core");
            layout(0, vec3, out var pos);
            layout(1, vec3, out var color);

            outs(vec4, out var vertexColor);

            gl_Position = vec(pos, 1.0f);

        })
        .SetFragmentShader(() =>
        {

        })
        .Build();
};

Window.Open();