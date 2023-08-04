﻿using DuckGL;
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

            gl_Position = vec(pos, 1.0f);
            outVar(vec4, "vertexColor", color);
        })
        .SetFragmentShader(() =>
        {
            version("330 core");
            var vertexColor = inVar(vec4, "vertexColor");

            gl_FragColor = vec(vertexColor, 1.0f);
        })
        .Build();
};

Window.Open();