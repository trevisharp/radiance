# Radiance

A library based on OpenTK which is based on OpenGL to program Shaders in C#.

# Table of Contents

 - [Overview](#overview)
 - [How to install](#how-to-install)
 - [Learn by examples](#learn-by-examples)
 - [Versions](#versions)
 - [Next Features](#next-features)

# Overview

Radiance is a library that can generate GLSL (The language of OpenGL) automatically only using C# code. Radiance manage OpenGL to create buffers, associate variables, compile shaders and control programs. With Radiance will can use OpenGL in high level without completly lose the high performance of OpenGL and GPU renderization.

# How to install

```bash
dotnet new console # Create project
dotnet add package radiance # Install Radiance
```

# Learn by examples

### Create a window for your apps

```cs
using Radiance;

Window.OnKeyDown += (input, modifier) =>
{
    // Test if active modifier has pressed like, alt, shift, ctrl.
    bool dontHasMod = (modifier & Modifier.ActiveModifier) == 0;
    if (input == Input.Escape && !dontHasMod)
        Window.Close();
};
// Or use:
// Window.CloseOn(Input.Escape);

Window.Open(false); // true or ignore parameter for fullscreen
```

### Draw simple objects

```cs
using Radiance;
using static Radiance.Utils; // utilities

var rect = Empty; // empty polygon
Window.OnLoad += () => 
    rect = Rect(
        Window.Width / 2,
        Window.Height / 2,
        0, 500, 500
    ); // use Window information to create polygons

Window.OnRender += () =>
    Kit.Fill(rect, red); // use renders to fill the polygon

Window.CloseOn(Input.Escape);
Window.Open();
```

### Create custom renders and see the generated GLSL code 

```cs
using Radiance;
using static Radiance.Utils;

var myRender = render(() =>
{
    verbose = true;
    clear(black);

    pos += center;
    
    var scale = x / width;
    color = (scale, 0, 1, 1);
    fill();
}).Curry(Rect(500, 500)); // fix parameters to render

bool isVisible = true;
Window.OnRender += myRender;

Window.OnKeyDown += (key, mod) =>
{
    if (key != Input.Space)
        return;
    
    if (isVisible)
        Window.OnRender -= myRender;
    else Window.OnRender += myRender;

    isVisible = !isVisible;
};

Window.CloseOn(Input.Escape);
Window.Open();
```

### Use OpenGL functions and interact with cursor

```cs
using Radiance;
using static Radiance.Utils;

var oncursor = render((cx, cy) =>
{
    verbose = true;
    pos += (0.5, 0.5, 0);
    pos *= (width, height, 1);

    var d = distance((x, y), (cx, cy));
    var s = (5.0 + 0.01 * sin(10 * t)) / d;
    color = (s, s, s, 1);
    fill();
});

float cx = 0f;
float cy = 0f;
Window.OnMouseMove += p => (cx, cy) = p;

var rect = Rect(1, 1);
Window.OnRender += () => oncursor(rect, cx, cy);

Window.CursorVisible = false;

Window.CloseOn(Input.Escape);
Window.Open();
```

### Fill complex polygons using triangulation

```cs
using Radiance;
using static Radiance.Utils;

var poly = Empty
    .Add(700, 200, 0)
    .Add(710, 210, 0)
    .Add(720, 230, 0)
    .Add(730, 300, 0)
    .Add(740, 220, 0)
    .Add(750, 230, 0)
    .Add(760, 270, 0)
    .Add(770, 200, 0)
    .Add(780, 300, 0)
    .Add(790, 220, 0)
    .Add(800, 250, 0)
    .Add(810, 260, 0)
    .Add(820, 230, 0)
    .Add(830, 280, 0)
    .Add(840, 350, 0)
    .Add(850, 280, 0)
    .Add(860, 300, 0)
    .Add(870, 290, 0)
    .Add(880, 200, 0)
    .Add(890, 400, 0)
    .Add(900, 150, 0);

Window.OnRender += () =>
    Kit.Fill(poly, red);

Window.CloseOn(Input.Escape);
Window.Open();
```

### Interact with yours renders

```cs
using Radiance;
using static Radiance.Utils;

float px = 0;
float py = 0;

var horMov = 0f;
var verMov = 0f;

var maxSpeed = 500;

Window.OnLoad += delegate
{
    px = Window.Width / 2 - 25;
    py = Window.Height / 2 - 25;
};

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
    
    px += horMov * dt;
    py += verMov * dt;
    
    if (horMov > 0)
        horMov -= maxSpeed * dt;
    else if (horMov < 0)
        horMov += maxSpeed * dt;

    if (verMov > 0)
        verMov -= maxSpeed * dt;
    else if (verMov < 0)
        verMov += maxSpeed * dt;
};

var simple = render((px, py) =>
{
    verbose = true;
    pos *= 50;
    pos += (px, py, 0);
    color = red;
    draw();
});
var rect = Rect(1, 1);

Window.OnRender += () => simple(rect, px, py);

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

### Use textures in your apps

```cs
using Radiance;
using static Radiance.Utils;

var myRender = render((img1, img2) =>
{
    verbose = true;
    clear(white);

    pos *= 400;
    pos += center;
    
    var point = (x / width, y / height);
    var text1 = texture(img1, point);
    var text2 = texture(img2, point);
    color = mix(text1, text2, (sin(t) + 1) / 2);
    fill();
});

var f1 = open("faustao1.png");
var f2 = open("faustao2.png");
var f3 = open("faustao3.jpg");
var faustao = myRender(Circle, f1);

var img = f2;
Window.OnRender += () => faustao(img);

Window.OnKeyDown += (key, mod) =>
{
    if (key == Input.Space)
        img = img == f2 ? f3 : f2;
};

Window.CloseOn(Input.Escape);
Window.Open();
```

### Use clock type to manage the time

```cs
using System.Collections.Generic;
using OpenTK.Compute.OpenCL;
using Radiance;
using static Radiance.Utils;

var shipRender = render((px) =>
{
    pos = 5 * pos + (px, height / 2, 0);
    color = red;
    fill();
});

var waveRender = render((px, py, size) =>
{
    pos = 34 * size * pos + (px, py, 0);
    color = white;
    draw();
});

float shipSpeed = 4f;
float shipPosition = 0f;
Window.OnRender += () =>
    shipRender(Circle, shipPosition);

var clkFrame = new Clock();
var clkWave = new Clock();
List<Clock> waveClocks = [];
Window.OnFrame += () =>
{
    shipPosition += shipSpeed * clkFrame.Time;
    shipSpeed += 3 * clkFrame.Time;
    clkFrame.Reset();

    if (clkWave.Time < 0.2)
        return;
    clkWave.Reset();

    var clk = new Clock();
    waveClocks.Add(clk);
    var origin = shipPosition;
    Window.OnRender += () =>
        waveRender(Circle, origin, Window.Height / 2, clk.Time);
};

Window.OnKeyDown += (k, m) =>
{
    if (k == Input.Space)
    {
        foreach (var clk in waveClocks)
            clk.ToogleFreeze();
        clkFrame.ToogleFreeze();
        clkWave.ToogleFreeze();
    }
};

Window.CloseOn(Input.Escape);
Window.Open();
```

# Versions

### Radiance v3.0.0
 - ![](https://img.shields.io/badge/new-green) Now, renders can be called insine another shaders.
 - ![](https://img.shields.io/badge/new-green) New default shaders on RenderKit.
 - ![](https://img.shields.io/badge/new-green) Add more functions like rand and noise.
 - ![](https://img.shields.io/badge/new-green) Add a Pipeline abstraction to speedup render by the union of Render objects in less calls.
 - ![](https://img.shields.io/badge/new-green) Add Text writting utilities.
 - ![](https://img.shields.io/badge/update-blue) Improve on internal abstractions and some break changes. Make Radiance more extensible.
 - ![](https://img.shields.io/badge/update-blue) Now, Shader Dependencies can generate code and add other dependencies on shader generation.
 - ![](https://img.shields.io/badge/removed-red) Remove some features on Polygon simplifying the abstraction.

### Radiance v2.4.0

 - ![](https://img.shields.io/badge/update-blue) Improve the use of OpenGL Primitives to improve some operations.
 - ![](https://img.shields.io/badge/update-blue) Improve the agregation of many poligons in a unique GPU call.
 - ![](https://img.shields.io/badge/bug%20solved-orange) Improve triangulation to avoid fail in more polygons.

### Radiance v2.3.1

 - ![](https://img.shields.io/badge/bug%20solved-orange) Fix a bug when we use 3d points and make a trinagulation.
  - ![](https://img.shields.io/badge/update-blue) Improve WindowClosedException message.
 - ![](https://img.shields.io/badge/new-green) Add vw and vh property in Utils class to get the real number of pixel of screen.

### Radiance v2.3.0

 - ![](https://img.shields.io/badge/bug%20solved-orange) Fix a bug when we use x, y, z Utilities to replace pos property and we lose the BufferDependence reference.
 - ![](https://img.shields.io/badge/update-blue) Improve the diversity of operations using VecN objects.
 - ![](https://img.shields.io/badge/update-blue) Add currying using float arrays.
 - ![](https://img.shields.io/badge/update-blue) Now cursor starts in Normal Visible mode.
 - ![](https://img.shields.io/badge/update-blue) Add more render function overloads.

### Radiance v2.2.0

 - ![](https://img.shields.io/badge/new-green) Clock object to easy management of time.
 - ![](https://img.shields.io/badge/new-green) Add Draw function in Render Kit.

### Radiance v2.1.2

 - ![](https://img.shields.io/badge/bug%20solved-orange) Fix a bug when render call renderization implementations with one parameter less instead make curry operation.
 - ![](https://img.shields.io/badge/bug%20solved-orange) Fix a bug when the render functions is callend outside OnRender event showing the incorrect message error.

### Radiance v2.1.1

 - ![](https://img.shields.io/badge/new-green) Add More properties in Utils class.
 - ![](https://img.shields.io/badge/bug%20solved-orange) Fix bug in tuple convention.
 - ![](https://img.shields.io/badge/bug%20solved-orange) Fix bug in Rect function.

### Radiance v2.0.0

 - ![](https://img.shields.io/badge/new-green) Add Release Shaders to speedup production apps.
 - ![](https://img.shields.io/badge/new-green) Add Conditional Rendering.
 - ![](https://img.shields.io/badge/new-green) Enable Currying in renders.
 - ![](https://img.shields.io/badge/new-green) Create Data Providers to increase the power of data structures.
 - ![](https://img.shields.io/badge/update-blue) New shader programming syntax.
 - ![](https://img.shields.io/badge/update-blue) Replace old Renders by a new and most powerfull Render abstraction.
 - ![](https://img.shields.io/badge/update-blue) Replace old Data abstraction by Polygon class and Vecs.
 - ![](https://img.shields.io/badge/update-blue) Improve generate code legibility and expression reuse.
 - ![](https://img.shields.io/badge/update-blue) Improve abstraction of shaders to improve and organization.
 - ![](https://img.shields.io/badge/bug%20solved-orange) Solving bugs in triangularization.
 - ![](https://img.shields.io/badge/removed-red) Remove old ShaderFunctions, Types and Data abstractions.

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