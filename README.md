# Radiance

An OpenGL/OpenTK-based library 2D foccused to program shaders easily in C#.

# Table of Contents

 - [Overview](#overview)
 - [How to install](#how-to-install)
 - [Learn by examples](#learn-by-examples)
 - [Versions](#versions)

# Overview

Radiance is a library that can generate GLSL (The language of OpenGL) automatically only using C# code. Radiance manage OpenGL to create buffers, associate variables, compile shaders and control programs. With Radiance will can use OpenGL in high level without completly lose the high performance of OpenGL and GPU renderization.

# How to install

```bash
dotnet new console # Create project
dotnet add package radiance # Install Radiance
```

# Learn by examples

In this section we added exemples that cover the principal features of Radiance.

### Create a simple fullscreen window

```cs
using Radiance;

Window.CloseOn(Input.Escape);
Window.Open();
```

### Create a custom render

```cs
using Radiance;
using static Radiance.Utils;

var myRender = render(() => {
    color = red;
    fill();

    color = white;
    draw();
});

var myPolygon = Polygons.FromData(
    (0, 0), (100, 0), (100, 100), (0, 100)
);

Window.OnRender += () => myRender(myPolygon);

Window.CloseOn(Input.Escape);
Window.Open();
```

### Create a render with parameters

```cs
using Radiance;
using static Radiance.Utils;

var myRender = render((r, g, b, a) => {
    color = (r, g, b, a);
    fill();
});

var myPolygon = Polygons.FromData(
    (0, 0), (100, 0), (100, 100), (0, 100)
);

// red = (1, 0, 0, 1), so can be used to set 4 parameters
// you can do that too:
// Window.OnRender += () => myRender(myPolygon, 1, 0, 0, 1);
Window.OnRender += () => myRender(myPolygon, red);

Window.CloseOn(Input.Escape);
Window.Open();
```

### Curry parameters

```cs
using Radiance;
using static Radiance.Utils;

var myRender = render((r, g, b, a) => {
    color = (r, g, b, a);
    fill();
});

var myPolygon = Polygons.FromData(
    (0, 0), (100, 0), (100, 100), (0, 100)
);

// mySquareRender every is called with the myPolygon value
// but still expect a 'r', 'g', 'b' and 'a' parameters
var mySquareRender = myRender(myPolygon);

// myRedRender no fix a polygon, using Utils.skip field
// but he fix the color red for (r, g, b, a) arguments
var myRedRender = myRender(skip, red);

// now we can call myRedRender with only myPolygon
Window.OnRender += () => myRedRender(myPolygon);

Window.CloseOn(Input.Escape);
Window.Open();
```


### Use built-in renders to simplify the work

```cs
using Radiance;
using static Radiance.Utils;

var myRender = render((r, g, b, a, size, dx, dy) => {
    zoom(size); // zoom in (0, 0)
    move(dx, dy);
    color = (r, g, b, a);
    fill();
});

Window.OnRender += () => myRender(
    Polygons.Square, // A square with 1x1 size on (0, 0) coridnate
    red, 100, 200, 200
);

Window.CloseOn(Input.Escape);
Window.Open();
```

### Transform the polygon as you want

```cs
using Radiance;
using static Radiance.Utils;

var myRender = render((r, g, b, a, size, dx, dy) => {
    zoom(size);
    move(dx, dy);
    // modify the position of polygon vertex
    pos = (pos.x, pos.y + pos.x / 5, pos.z);
    // use x, y, z, variables to define each pixel color
    color = (r, g, x / 300, a);
    fill();
});

Window.OnRender += () => myRender(
    Polygons.Square,
    red, 100, 200, 200
);

Window.CloseOn(Input.Escape);
Window.Open();
```

### Use Utils.t to create amazing animations

```cs
using Radiance;
using static Radiance.Utils;

var myRender = render((r, g, b, a, size, dx, dy) => {
    zoom(size);
    move(dx + 10 * t, dy + 10 * t);
    color = (r, g, b, a);
    fill();
});

Window.OnRender += () => myRender(
    Polygons.Square,
    red, 100, 200, 200
);

Window.CloseOn(Input.Escape);
Window.Open();
```

### Use GPU functions to create effects fast

```cs
using Radiance;
using static Radiance.Utils;

var myRender = render((size, dx, dy) => {
    zoom(size);
    move(dx, dy);
    // mix recive two values and a coeficient between 0 and 1
    // and chooses a mix of the values using this coeficient
    // 0 = red, 1 = blue, 0.5 = (red + blue) / 2

    // sin is a trigonometric function that return a value
    // between -1 and 1. cos functions exists to, but
    // sin(0) = 0 and cos(0) = 1.
    var coef = (sin(5 * t) + 1) / 2;
    color = mix(red, blue, coef);
    fill();
});

Window.OnRender += () => myRender(
    Polygons.Square,
    100, 200, 200
);

Window.CloseOn(Input.Escape);
Window.Open();
```

### Use trigonometric functions to create loop behaviours

```cs
using Radiance;
using static Radiance.Utils;

var myRender = render(() => {
    zoom(100 + 50 * sin(5 * t));
    centralize();
    move(200 * cos(2 * t), 200 * sin(2 * t));
    color = red;
    fill();
});

Window.OnRender += () => myRender(Polygons.Square);

Window.CloseOn(Input.Escape);
Window.Open();
```

### Create amazing effects with Radiance

```cs
using Radiance;
using static Radiance.Utils;

var myRender = render(() => {
    var center = (width / 2, height / 2);
    var pixel = (x, y);
    var dist = distance(pixel, center);
    var invDist = 100 / dist;
    var light = min(invDist, 1);
    color = (light, light, light, 1f);
    fill();
});

// 1.Your polygon is the entire screen now
// 2.We using a currying operation. Now render has the
// first parameter fixed on Polygons.Screen
// this is interesting because create a polygon on OnRender
// can produce wrong behaviours or performance loss
// 3.We make the operation in OnLoad to grant that the Screen
// already be initialized when the polygon is created.
// In other case we can do myRender = myRender(Polygons.Rect(200, 200))
// outside OnLoad function.
Window.OnLoad += () => myRender = myRender(Polygons.Screen);
Window.OnRender += () => myRender();

Window.CloseOn(Input.Escape);
Window.Open();
```

# Versions

### Radiance v3.0.0

 - ![](https://img.shields.io/badge/new-green) Now, renders can be called insine another shaders.
  - ![](https://img.shields.io/badge/new-green) Use of many buffer has parameters for renders is allowed.
 - ![](https://img.shields.io/badge/new-green) Move RenderKit content to Utils with more renders.
 - ![](https://img.shields.io/badge/new-green) Add more functions like rand and noise.
 - ![](https://img.shields.io/badge/new-green) Add a Pipeline abstraction to speedup render by the union of Render objects in less calls.
 - ![](https://img.shields.io/badge/update-blue) Improve on internal abstractions and some break changes. Make Radiance more extensible.
 - ![](https://img.shields.io/badge/update-blue) Now, Shader Dependencies can generate code and add other dependencies on shader generation.
 - ![](https://img.shields.io/badge/update-blue) Polygon very simplified and immutable every time.
 - ![](https://img.shields.io/badge/bug%20solved-orange) Fix a bug when we try to use dt in OnFrame and close the program abruptly.
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

# TODO
- Use BufferSubData to improve update performance.
- Improve Render class abstractions allowing extensibiltiy and customization.
- Validate depths of input of renders consistence.
- Improve polygon initialize to avoid bugs when a polygon is created on OnRender.
- Text Renders.
- Improve ShaderObjects Resources.
- Improve the use of more than one window.
- Create the Graphics class for call simple draw operations.
- Add non-monotone polygons triangularization.
- Make z-index between 0 to 1000 to make layer-based organization more simple.
- Allow basic transformations like z-index adjustment customizations.
- Improve variable generation name to improve shader reutilization.
- Avaliate dependency cycles on GLSLGenerator.