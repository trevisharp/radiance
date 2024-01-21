# Radiance

A library based on OpenTK which is based on OpenGL to program Shaders in C#.

# Table of Contents

 - [Overview](#overview)
 - [How to install](#how-to-install)
 - [Features (For OpenGL Programers)](#features-for-opengl-programers)
 - [Concepts](#concepts)
 - [Examples](#examples)
 - [Versions](#versions)
 - [Next Features](#next-features)

# Overview

Radiance is a library that can generate GLSL (The language of OpenGL) automatically only using C# code. Radiance manage OpenGL to create buffers, associate variables, compile shaders and control programs. With Radiance will can use OpenGL in high level without completly lose the high performance of OpenGL and GPU renderization.

# How to install

```bash
dotnet new console # Create project
dotnet add package radiance # Install Radiance
```

# Features (For OpenGL Programers)

See this examples that contains all Radiance features:

### Create a window for your apps

```cs
using Radiance;

Window.OnKeyDown += (input, modifier) =>
{
    // Test if active modifier has pressed like, alt, shift, ctrl.
    bool dontHasMod = (modifier & Modifier.ActiveModifier) == 0;
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

### Get cursor position and use all potential of Radiance

```cs
using Radiance;
using Radiance.Types;
using static Radiance.RadianceUtils;

gfloat x = 0f;
gfloat y = 0f;

var screen = data(
    n, i, i + j,
    n, j, i + j
);

Window.OnRender += r =>
{
    r.Verbose = true;
    r.FillTriangles(screen
        .transform(v => (v.x * width, v.y * height, v.z))
        .colorize(v => 
        {
            var point = (v.x * width, v.y * height, v.z);
            var cursor = (x, y, 0);
            var d = distance(point, cursor);
            var s = (5.0 + 0.01 * sin(10 * t)) / d;
            return (s, s, s, 0);
        })
    );
};

Window.OnMouseMove += p => (x, y) = p;

Window.CursorVisible = false;

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

### Draw Textures

```cs
﻿using Radiance;
using static Radiance.RadianceUtils;

var myRender = render(r =>
{
    var img = open("pain.png");
    var mod = 400 * sin(t / 5);
    r.Clear(white);
    r.Verbose = true;
    r.FillTriangles(circle
        .triangules()
        .transform(v => (mod * v.x + width / 2, mod * v.y + height / 2, v.z))
        .colorize(v => texture(img, (mod * v.x / width + 0.5f, mod * v.y / height + 0.5f)))
    );
});

Window.OnRender += myRender;

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

# Examples

### Simple Game
```cs
using System;

using Radiance;
using Radiance.Types;
using static Radiance.Window;
using static Radiance.RadianceUtils;

Player player = new Player();

Cursor cursor = new Cursor();

Ligth ligth1 = new Ligth();
Ligth ligth2 = new Ligth();
Ligth ligth3 = new Ligth();
Ligth ligth4 = new Ligth();
Ligth ligth5 = new Ligth();
Ligth ligth6 = new Ligth();

OnFrame += delegate
{
    player.TryCapture(
        ligth1, ligth2, ligth3, 
        ligth4, ligth5, ligth6
    );
    Console.WriteLine(Fps);
};

CloseOn(Input.Escape);

CursorVisible = false;

Open();

public class Ligth
{
    public float X => x.Value;
    public float Y => y.Value;

    gfloat x = -1;
    gfloat y = -1;
    gfloat captured = 1; 

    float nextx = -1;
    float nexty = -1;

    Player player = null;

    public Ligth()
    {
        render(r =>
        {
            r.FillTriangles(
                data(n, i, i + j, n, j, i + j)
                .transform(v => (50 * v.x + x, 50 * v.y + y, v.z))
                .colorize(v => 
                {
                    var d = distance(v, (0.5, 0.5, 0));
                    var s = 0.001 / (d * d);
                    return (s / captured, s, s / captured, s);
                })
            );
        }).Show();

        OnFrame += delegate
        {
            Move();
        };
    }

    public void Move()
    {
        if (x.Value == -1)
        {
            x = Random.Shared.NextSingle() * Window.Width;
            y = Random.Shared.NextSingle() * Window.Height;
        }

        if (nextx == -1 || nexty == -1)
        {
            nextx = player is null ?
                Random.Shared.NextSingle() * Window.Width :
                player.X + Random.Shared.NextSingle() * 50 - 25;
            nexty = player is null ?
                Random.Shared.NextSingle() * Window.Height :
                player.Y + Random.Shared.NextSingle() * 50 - 25;
        }

        var dx = nextx - x.Value;
        var dy = nexty - y.Value;
        var d = MathF.Sqrt(dx * dx + dy * dy);

        if (d < (player is not null ? 1 : 10))
        {
            nextx = nexty = -1;
            return;
        }

        var vx = dx / d + 4 * (Random.Shared.NextSingle() - .5f);
        var vy = dy / d + 4 * (Random.Shared.NextSingle() - .5f);

        x += 50 * vx * dt * (player is not null ? 6 : 1);
        y += 50 * vy * dt * (player is not null ? 6 : 1);
    }

    public void Capture(Player player)
    {
        nextx = nexty = -1;
        this.player = player;
        captured = 10;
    }
}

public class Cursor
{
    gfloat x = -1;
    gfloat y = -1;

    public Cursor()
    {
        render(r =>
        {
            r.FillTriangles(
                data(n, i, i + j, n, j, i + j)
                .transform(v => (50 * v.x + x, 50 * v.y + y, v.z))
                .colorize(v => 
                {
                    var d = distance(v, (0.5, 0.5, 0));
                    var s = 0.001 / (d * d);
                    return (s, 0, 0, s);
                })
            );
        }).Show();

        OnMouseMove += p => (x, y) = p;
    }
}

public class Player
{
    public float X => x.Value;
    public float Y => y.Value;

    gfloat x = 0;
    gfloat y = 0;

    float ox;
    float oy;

    public Player()
    {
        render(r => {
            r.Draw(
                rect(0, 0, 50, 50)
                .transform(v => (v.x + x, v.y + y, 0))
            );
        }).Show();

        OnMouseMove += p => (ox, oy) = p;

        OnFrame += delegate
        {
            var dx = ox - x.Value;
            var dy = oy - y.Value;
            var d = MathF.Sqrt(dx * dx + dy * dy);

            if (d < 2)
                return;

            var vx = dx / d;
            var vy = dy / d;

            x += 150 * vx * dt;
            y += 150 * vy * dt;
        };
    }

    public void TryCapture(params Ligth[] ligths)
    {
        foreach (var ligth in ligths)
            TryCapture(ligth);
    }

    public void TryCapture(Ligth ligth)
    {
        if (ligth.X > x.Value + 25 || ligth.Y > y.Value + 25 ||
            ligth.X < x.Value - 25 || ligth.Y < y.Value - 25)
            return;
        
        ligth.Capture(this);
    }

}
```

# Versions

### Radiance v2.0.0 (coming soon)

 - ![](https://img.shields.io/badge/new-green) Add Text writting.
 - ![](https://img.shields.io/badge/new-green) Add Release Shaders to speedup production apps.
 - ![](https://img.shields.io/badge/new-green) Add Conditional Rendering.
 - ![](https://img.shields.io/badge/new-green) Create Data Providers to increase the power of data structures.
 - ![](https://img.shields.io/badge/update-blue) Replace old Renders by a new and most powerfull Render abstraction.
 - ![](https://img.shields.io/badge/update-blue) Replace old Data abstraction by Polygon class and Vecs.
 - ![](https://img.shields.io/badge/update-blue) Improve generate code legibility and expression reuse.
 - ![](https://img.shields.io/badge/update-blue) Improve abstraction of shaders to improve and organization.
 - ![](https://img.shields.io/badge/update-blue) Improve triangularization performance.
 - ![](https://img.shields.io/badge/bug%20solved-orange) Solving bugs in triangularization.
 - ![](https://img.shields.io/badge/removed-red) Remove old ShaderFunctions and Data abstractions.

### Radiance v1.9.0

 - ![](https://img.shields.io/badge/new-green) Active Blending.
 - ![](https://img.shields.io/badge/new-green) Add texture system and multi-texture system.
 - ![](https://img.shields.io/badge/new-green) Add more radiance utils elements.
 - ![](https://img.shields.io/badge/new-green) Added Triangularization algorithms for x-monotone polygons.
 - ![](https://img.shields.io/badge/new-green) Add a Zero time to know the DateTime of initial t field in RadianceUtils.
 - ![](https://img.shields.io/badge/update-blue) Update OpenTK version to 4.8.2.
 - ![](https://img.shields.io/badge/bug%20solved-orange) Disable some warning from base code.
 - ![](https://img.shields.io/badge/bug%20solved-orange) Solve bugs to improve reutilization of shaders and programns.
 - ![](https://img.shields.io/badge/bug%20solved-orange) Solve bugs on float value string format in the code generation.
 - ![](https://img.shields.io/badge/bug%20solved-orange) Solved bug that crash the shader when we use a gfloat multiple times in the FragmentShader.

### Radiance v1.5.0

 - ![](https://img.shields.io/badge/new-green) Hide Cursor Feature.
 - ![](https://img.shields.io/badge/new-green) Add system to add and remove render functions.
 - ![](https://img.shields.io/badge/new-green) Create a class BaseData : IData to simplify IData's implementations.
 - ![](https://img.shields.io/badge/update-blue) Change all structure about RenderFunctions and introduce a IRender interface to all renders and create a RenderQueue.
 - ![](https://img.shields.io/badge/update-blue) Create system to improve data administration and replace old system of data management.
 - ![](https://img.shields.io/badge/update-blue) Now, change data not affect the draw and data become imutable again. In next versions, Radiance will provide new ways to talk with data and the they changes.
 - ![](https://img.shields.io/badge/update-blue) Improve performance of Data Structures.

### Radiance v1.4.0

 - ![](https://img.shields.io/badge/new-green) Program reusing if shaders are same in diferent draw calls.
 - ![](https://img.shields.io/badge/new-green) Rect and Ellip shafe functions in RadianceUtils.
 - ![](https://img.shields.io/badge/update-blue) Global system types updates.

### Radiance v1.3.0

 - ![](https://img.shields.io/badge/new-green) Cursor input events added.
 - ![](https://img.shields.io/badge/new-green) Improve use of data turning into a mutable data and avoid regenerating buffer if the data not change.
 - ![](https://img.shields.io/badge/new-green) Improve use of OpenGL resources and reutilizations.
 - ![](https://img.shields.io/badge/update-blue) Add modifier systems to keyboard input.
 - ![](https://img.shields.io/badge/update-blue) Improve Verbose mode outputs.

### Radiance v1.2.0

 - ![](https://img.shields.io/badge/new-green) 'dt' Delta time variable and fps control.
 - ![](https://img.shields.io/badge/new-green) Global variables system

### Radiance v1.1.0

 - ![](https://img.shields.io/badge/new-green) Colored Vectors data type added.
 - ![](https://img.shields.io/badge/new-green) Auto outputs and inputs betwen Shader Pipeline.
 - ![](https://img.shields.io/badge/update-blue) Data System Reworked.
 - ![](https://img.shields.io/badge/update-blue) More itens in Radiance Utils.

### Radiance v1.0.0

 - ![](https://img.shields.io/badge/new-green) Window static Class to manage screen
 - ![](https://img.shields.io/badge/new-green) Shader object system that can converts C# to GLSL
 - ![](https://img.shields.io/badge/new-green) Data Types like Vector, Vectors, and transformed types.
 - ![](https://img.shields.io/badge/new-green) Methods FillTriangles and Draw (Line Loop) in render functions.
 - ![](https://img.shields.io/badge/new-green) Many functions of GLSL like cos, sin distance, round, smoothstep and others...
 - ![](https://img.shields.io/badge/new-green) RadianceUtils static class with all util operations and data.
 - ![](https://img.shields.io/badge/new-green) Dependece system with auto add uniforms and variables like width, heigth and the time in the app (named t) to use in implementations.

# Next Features

- README: Add the section 'For not OpenGL programers'
- README: Add the section 'What not to do'
- README: Add the section 'Advanced operations' teaching how to create custom data types
- README: Add the section 'Template Project' with a template for the project
- Enable Vertex Object Array reutilization in OpenGLManager
- Add matrix Transformations
- Add Data types to have more generic overloads
- Solve bugs with gfloat operations
- Add more g-types
- Allow and Improve multi transform call
- Improve conversion between g-types and shader types
