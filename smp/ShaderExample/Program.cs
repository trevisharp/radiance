using DuckGL;
using static DuckGL.Shaders;

Graphics g = null;

Window.OnLoad += delegate
{
    g = Window
        .CreateGraphics()
        .SetVertexShader(() =>
        {

        })
        .SetFragmentShader(() =>
        {
            
        })
        .Build();
};

Window.Open();