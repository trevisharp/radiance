# Radiance

An OpenGL/OpenTK-based library 2D foccused to program shaders easily in C#.

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

Coming soon...

# Versions

### Radiance v3.0.0
 - ![](https://img.shields.io/badge/new-green) Now, renders can be called insine another shaders.
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

# TODO 3.0
- Improve renders abstractions allowing extensibiltiy and customization.
- Implements the Utils.onPolygon function.
- Consider move factories to buffers and improve buffer abstractions and use.
- Allow data update/mutability on onVertex and onPolygon funcs.
- Use BufferSubData to improve update performance.

- Text Renders.

- Improve ShaderObjects Resources.

- Improve the use of more than one window.
- Create the Graphics class for call simple draw operations.

- Add non-monotone polygons triangularization.

- Make z-index between 0 to 1000 to make layer-based organization more simple.
- Allow basic transformations like z-index adjustment customizations.