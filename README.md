# Radiance

A library based on OpenTK which is based on OpenGL to program Shaders in C#.

# How to use

```bash
dotnet new console # Create project
dotnet add package radiance # Install Radiance
```

# Features

See this examples that contains all Radiance features:

### Create a window for your apps

```cs
using Radiance;

Window.OnKeyDown += (input, modifier) =>
{
    // Test if active modifier has pressed like, alt, shift, ctrl.
    bool dontHasMod = (m & Modifier.ActiveModifier) == 0;
    if (input == Input.Escape && !dontHasMod)
        Window.Close();
}
// Or use:
// Window.CloseOn(Input.Escape);

Window.Open(false); // true or ignore parameter for fullscreen
```

### Use C# to generate OpenGL Shaders with auto parameters

```cs
using Radiance;
using static Radiance.RadianceUtils;

var region = data(
    (-1, -1, 0), // or -i -j
    (+1, -1, 0), // or +i -j
    (-1, +1, 0), // or -i +j

    (+1, +1, 0), // or +i +j
    (+1, -1, 0), // or +i -j
    (-1, +1, 0)  // or -i +j
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

### Global variables automatically turned into a uniform

```cs
using Radiance;
using Radiance.Types;
using static Radiance.RadianceUtils;

// init global variable
gfloat x = 0;
gfloat y = 0;

var horMov = 0f;
var verMov = 0f;

var maxSpeed = 500;

var region = data(n, i, i + j, j);

Window.OnLoad += delegate
{
    // Use global variables normally
    x = Window.Width / 2 - 25;
    y = Window.Height / 2 - 25;
};

// Update values of global values out of OnRender
Window.OnFrame += delegate
{
    if (horMov > maxSpeed)
        horMov = maxSpeed;
    else if (horMov < -maxSpeed)
        horMov = -maxSpeed;

    if (verMov > maxSpeed)
        verMov = maxSpeed;
    else if (verMov < -maxSpeed)
        verMov = -maxSpeed;
    
    // dt = deltaTime = time between frames
    // use dt to avoid speed based on fps
    x += horMov * dt;
    y += verMov * dt;
    
    if (horMov > 0)
        horMov -= maxSpeed * dt;
    else if (horMov < 0)
        horMov += maxSpeed * dt;

    if (verMov > 0)
        verMov -= maxSpeed * dt;
    else if (verMov < 0)
        verMov += maxSpeed * dt;
};

Window.OnRender += r =>
{
    r.Draw(region
        .transform(v => (v.x * 50 + x, v.y * 50 + y, v.z))
    );
};

// Update global variables in others events
Window.OnKeyDown += (input, modifier) =>
{
    if (input == Input.D)
        horMov = maxSpeed;
    
    if (input == Input.A)
        horMov = -maxSpeed;
    
    if (input == Input.W)
        verMov = maxSpeed;

    if (input == Input.S)
        verMov = -maxSpeed;
};

Window.CloseOn(Input.Escape);

Window.Open();
```

### Use multi Draw operations and analize the process with Verbose Mode

```cs
using Radiance;
using Radiance.Types;
using static Radiance.RadianceUtils;

var rect = data(
    n, i, i + j,
    n, j, i + j
);

Window.OnRender += r =>
{
    r.Verbose = true;
    float N = 40;
    // Local variables are considered a uniform. You can generate a single
    // shader code using them. To update the values of l-type variables you
    // need ouse x++, x-- or x.Update(value). Use x.Value to access the value
    // of variable.
    for (lfloat i = 0; i < N; i++)
    {
        // Create many prograns/Shaders
        r.FillTriangles(rect
            .transform(v => (v.x * 20 * (N - i), v.y * 20 * (N - i), 0))
            .colorize(i / N, 0, 0)
        );
    }
};

Window.CloseOn(Input.Escape);

Window.Open();
```

### Edit you data state and update in the screen

```cs
using Radiance;
using static Radiance.RadianceUtils;

var w = i;
var h = j;
var end = i + j;

var region = data(
    n, w, end,
    n, h, end
);

Window.OnRender += r =>
{
    r.Clear(black);

    r.FillTriangles(region
        .colorize(red)
    );
};

Window.OnFrame += () =>
{
    end.x++;
    end.y++;
    w.x++;
    h.y++;
    region.HasChanged();
};

Window.CloseOn(Input.Escape);

Window.Open();
```

### Get cursor position and use all potential of Radiance

```cs
using Radiance;
using static Radiance.RadianceUtils;

var x = 0f;
var y = 0f;

var cursor = i + j;

var region = data(
    n | black, i | black, cursor | white,
    n | black, j | black, cursor | white,

    2 * j | black, j | black, cursor | white,
    2 * j | black, 2 * j + i | black, cursor | white,

    2 * i | black, 2 * i + j | black, cursor | white,
    2 * i | black, i | black, cursor | white,

    2 * i + 2 * j | black, 2 * i + j | black, cursor | white,
    2 * i + 2 * j | black, 2 * j + i | black, cursor | white
);

Window.OnRender += r =>
{
    r.FillTriangles(region
        .transform((v, c) => (width * v.x / 2, height * v.y / 2, 0))
        .colorize((v, c) => c)
    );
};

Window.OnFrame += delegate
{
    cursor.x = 2 * x / Window.Width;
    cursor.y = 2 * y / Window.Height;
    region.HasChanged();
};

Window.OnMouseMove += p => (x, y) = p;

Window.CloseOn(Input.Escape);

Window.Open();
```

### Generate simple shapes easily

```cs
using Radiance;
using static Radiance.RadianceUtils;

var r1 = rect(50, 50, 200, 100);
var r2 = ellip(250, 150, 200, 50);
var r3 = ellip(350, 250, 50, 50, 6);

Window.OnRender += r =>
{
    r.Draw(r1);
    r.Draw(r2);
    r.Draw(r3);
};

Window.CloseOn(Input.Escape);

Window.Open();
```

### Add and Remove RenderFunctions

```cs
using Radiance;
using static Radiance.RadianceUtils;

var render1 = render(r =>
{
    r.Draw(
        rect(50, 50, 50, 50)
        .colorize(v => {
            var scale = (v.x - 50) / 50;
            return (scale, 0, 1, 1);
        })
    );
});

var render2 = render(r =>
{
    r.Draw(
        rect(100, 100, 50, 50)
        .colorize(v => {
            var scale = (v.y - 100) / 50;
            return (0, scale, 1, 1);
        })
    );
});

Window.OnRender += render1; // Or obj1.Show();
Window.OnRender += render2; // Or obj2.Show();

Window.OnKeyDown += (key, mod) =>
{
    switch (key)
    {
        case Input.A:
            render1.Toggle();
            break;

        case Input.S:
            render2.Toggle();
            break;
        
        case Input.D:
            render1.SoftHide();
            render2.SoftHide();
            break;
    }
};

Window.CloseOn(Input.Escape);

Window.Open();
```

# Concepts

### Shaders

A Shader is a basic concept of OpenGL. He is a program that runs in GPU and can process position of
vertex data and color of the pixels in the screen. Radiance generate Shaders code using a language of
OpenGL the GLSL. We learn more about Shaders in next concepts.

### Shader Object

The Shader Object do not is a concept that realy need be understanded to use Radiance, but is a good
concept to understand how the tecnology works. A Shader Object represetns a data that can be exist in
a Shader implementation. When we sum two Shader Objects doing "a + b" actually we just generating a 
expression in GLSL that execute in GPU in the future.

### Shader Dependence

When we use some objects that can be change betwen frames we generaly use Shader Dependences. These objects
are referenced by Shader Objects and can be added in Shaders like a dependence of data in program. For example,
using "using static Radiance.RadianceUtils" you can use the "t" variable: A Shader Dependence of time. Using in
yours expressions added a uniform, a input variable, in the shaders. Automatically, the time of application will
be sent to GPU and used by implementations.

### Shader Outputs

We will always have two shaders is our program: A Vertex Shader, that process positions of data and a Fragment
Shader, that process the colors of pixels. Therefore, exists a pipeline in OpenGL where we need declare output
and input betwen these two shaders. Radiance will make this process automatically betwen Shader Outputs object.

### IData

IData is a interface implemented by all data types. These data types have the real data of application and can
be use like a Shader Object with a dependence. Every data type have a default implementation to Vertex Shader
and Fragment Shader. Using transform function you can update the Vertex Shader. Using colorize function you
can update the Fragment Shader.

### IRenders

Every function added in the OnRender Event do not be caled every frame, actually. In reality, the function will
be transformed in a new function that transform our code in calls of OpenGL library and Shader pre computated.
In this way, our function will be executed only one time, in project setup and never execute again. This is
important to understand that some implementations do not work like expected. We need use render functions with
carefoul to avoid unwanted behaviors.

### Radiance Types

A real value that exist in your C# Program and maybe sended to GPU betwen uniform's can be represented with 
Radiance Types, like gfloat and lfloat. A gfloat or lfloat variable will be transormed into a uniform 
automatically. Radiance types can be global ou local. On loops, use radiance types to avoid create a high
quantity of shaders that are similiar but not equal.

# Versions

### Radiance v1.5.0 (Coming soon)

 - ![](https://img.shields.io/badge/new-green) Create a class BaseData : IData to simplify IData's implementations.
 - ![](https://img.shields.io/badge/update-orange) Improve performance of Data Structures.
 - ![](https://img.shields.io/badge/new-green) Add system to add and remove render functions.
 - ![](https://img.shields.io/badge/update-orange) Change all structure about RenderFunctions and introduce a IRender interface to all renders and create a RenderQueue.
 - ![](https://img.shields.io/badge/update-orange) Create system to improve data administration and replace old system of data management.

### Radiance v1.4.0

 - ![](https://img.shields.io/badge/update-orange) Global system types updates.
 - ![](https://img.shields.io/badge/new-green) Program reusing if shaders are same in diferent draw calls.
 - ![](https://img.shields.io/badge/new-green) Rect and Ellip shafe functions in RadianceUtils.

### Radiance v1.3.0

 - ![](https://img.shields.io/badge/new-green) Cursor input events added.
 - ![](https://img.shields.io/badge/update-orange) Add modifier systems to keyboard input.
 - ![](https://img.shields.io/badge/update-orange) Improve Verbose mode outputs.
 - ![](https://img.shields.io/badge/new-green) Improve use of data turning into a mutable data and avoid regenerating buffer if the data not change.
 - ![](https://img.shields.io/badge/new-green) Improve use of OpenGL resources and reutilizations.

### Radiance v1.2.0

 - ![](https://img.shields.io/badge/new-green) 'dt' Delta time variable and fps control.
 - ![](https://img.shields.io/badge/new-green) Global variables system

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

# Next Features

- README: Add the section 'What not to do'
- README: Add the section 'Advanced operations' teaching how to create custom data types
- README: Add the section 'Big Examples' with complex examples
- README: Add the section 'Template Project' with a template for the project
- Create Data Providers to increase the power of data structures.
- Fill function on RenderOperations that triangularize polygons automatically
- Add matrix Transformations
- Add Textures
- Add Conditional Rendering
- Add Data types to have more generic overloads
- Solve bugs with gfloat operations
- Add more g-types
- Allow and Improve multi transform call
- Improve conversion between g-types and shader types