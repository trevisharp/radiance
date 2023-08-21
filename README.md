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
var region = data(
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

    r.Clear(white);
    
    r.FillTriangles(region
        .transform(v => center + size * v) // operation done on the GPU
        .colorize(blue) // operation done on the GPU
    );
};

Window.CloseOn(Input.Escape);

Window.Open();
```

### Use OpenGL resources in C#

```cs
using Radiance;
using static Radiance.RadianceUtils;

var region = data(
    (-1, -1, 0),
    (+1, -1, 0),
    (-1, +1, 0),

    (+1, +1, 0),
    (+1, -1, 0),
    (-1, +1, 0)
);

var border = data(
    (-1, -1, 0),
    (+1, -1, 0),
    (+1, +1, 0),
    (-1, +1, 0)
);

Window.OnRender += r =>
{
    var size = 50 + 20 * cos(5 * t);
    var center = (width / 2, height / 2, 0);

    r.Clear(white);
    
    r.FillTriangles(region
        .transform(v => center + size * v)
        .colorize(cos(t) + 1, sin(t) + 1, 0)
    );

    r.Draw(border
        .transform(v => center + size * v)
        .colorize(black)
    );
};

Window.CloseOn(Input.Escape);

Window.Open();
```

### Use custom data types and execute all the OpenGL pipeline

```cs
using Radiance;
using static Radiance.RadianceUtils;

// Colored Vectors
var region = data(
    n | magenta,
    i | cyan,
    i + j | magenta,

    n | magenta,
    j | cyan,
    i + j | magenta
);

Window.OnRender += r =>
{
    // Work with variables auto added in shaders
    r.FillTriangles(region
        .transform((v, c) => (width * v.x, height * v.y, v.z))
        .colorize((v, c) => c) // auto outputs in vertex and inputs in fragment
    );
};

Window.CloseOn(Input.Escape);

Window.Open();
```

# Versions

### Radiance v1.1.0

 - ![](https://img.shields.io/badge/new-green) Colored Vectors data type added.
 - ![](https://img.shields.io/badge/update-orange) Data System Reworked.
 - ![](https://img.shields.io/badge/new-green) Auto outputs and inputs betwen Shader Pipeline.
 - ![](https://img.shields.io/badge/update-orange) More itens in Radiance Utils.

### Radiance v1.0.0

 - ![](https://img.shields.io/badge/new-green) Window static Class to manage screen
 - ![](https://img.shields.io/badge/new-green) Shader object system that can converts C# to GLSL
 - ![](https://img.shields.io/badge/new-green) Data Types like Vector, Vectors, and transformed types.
 - ![](https://img.shields.io/badge/new-green) Methods FillTriangles and Draw (Line Loop) in render functions.
 - ![](https://img.shields.io/badge/new-green) Many functions of GLSL like cos, sin distance, round, smoothstep and others...
 - ![](https://img.shields.io/badge/new-green) RadianceUtils static class with all util operations and data.
- ![](https://img.shields.io/badge/new-green) Dependece system with auto add uniforms and variables like width, heigth and the time in the app (named t) to use in implementations.