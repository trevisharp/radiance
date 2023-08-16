# Radiance

A library based on OpenTK which is based on OpenGL to program Shaders in C#.

# How to use install

```bash
dotnet new console # Create project
dotnet add package radiance #install Radiance
```

# Tutorials, Examples and Features

### Create a window for your apps

```cs
using Radiance;

Window.OnKeyDown += key =>
{
    if (key == Input.Escape)
        Window.Close();
}
// Or use:
// Window.CloseOn(Input.Escape);

Window.Open(false); // or ignore parameter for fullscreen
```

### Use C# to generate OpenGL Shaders with auto parameters

```cs
using Radiance;
using static Radiance.RadianceUtils;

// immutable data
var region = vecs(
    (-1, -1, 0),
    (+1, -1, 0),
    (-1, +1, 0),

    (+1, +1, 0),
    (+1, -1, 0),
    (-1, +1, 0)
);

Window.OnRender += r =>
{
    var size = 50;
    var center = (width / 2, height / 2, 0); // create uniforms automatically

    r.Clear(Color.White);
    
    r.FillTriangles(region
        .transform(v => center + size * v) // operation done on the GPU
        .colorize(Color.Blue) // operation done on the GPU
    );
};

Window.CloseOn(Input.Escape);

Window.Open();
```

### Use OpenGL resources in C#

```cs
using Radiance;
using static Radiance.RadianceUtils;

var region = vecs(
    (-1, -1, 0),
    (+1, -1, 0),
    (-1, +1, 0),

    (+1, +1, 0),
    (+1, -1, 0),
    (-1, +1, 0)
);

var border = vecs(
    (-1, -1, 0),
    (+1, -1, 0),
    (+1, +1, 0),
    (-1, +1, 0)
);

Window.OnRender += r =>
{
    var size = 50 + 20 * cos(5 * t);
    var center = (width / 2, height / 2, 0);

    r.Clear(Color.White);
    
    r.FillTriangles(region
        .transform(v => center + size * v)
        .colorize(cos(t) + 1, sin(t) + 1, 0, 1)
    );

    r.Draw(border
        .transform(v => center + size * v)
        .colorize(Color.Black)
    );
};

Window.CloseOn(Input.Escape);

Window.Open();
```

# Versions

### Radiance v1.0.0

 - ![](https://img.shields.io/badge/new-green) Window static Class to manage screen
 - ![](https://img.shields.io/badge/new-green) Shader object system that can converts C# to GLSL
 - ![](https://img.shields.io/badge/new-green) Data Types like Vector, Vectors, and transformed types.
 - ![](https://img.shields.io/badge/new-green) Methods FillTriangles and Draw (Line Loop) in render functions.
 - ![](https://img.shields.io/badge/new-green) Many functions of GLSL like cos, sin distance, round, smoothstep and others...
 - ![](https://img.shields.io/badge/new-green) RadianceUtils static class with all util operations and data.
- ![](https://img.shields.io/badge/new-green) Dependece system with auto add uniforms and variables like width, heigth and the time in the app (named t) to use in implementations.