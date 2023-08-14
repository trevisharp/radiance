# Radiance

A library based on OpenTK which is based on OpenGL to program Shaders in C#.

# Tutorials and Examples

## Create a Fullscreen window for your apps

```cs
using Radiance;

Window.OnKeyDown += key =>
{
    if (key == Input.Escape)
        Window.Close();
}
// Or use:
// Window.CloseOn(Input.Escape);

Window.Open();
```

# Versions

### Radiance v1.0.0 (coming soon)

### Radiance v0.2.0

 - ![](https://img.shields.io/badge/updated-green) Graphics Class
 - ![](https://img.shields.io/badge/new-green) GraphicsBuilder Class
 - ![](https://img.shields.io/badge/new-green) Color Record
 - ![](https://img.shields.io/badge/new-green) ColoredVertex Record
 - ![](https://img.shields.io/badge/new-green) Shaders Class
 - ![](https://img.shields.io/badge/new-green) ShaderSupport Namespace
    - ![](https://img.shields.io/badge/new-green) ShaderContext Class
    - ![](https://img.shields.io/badge/new-green) ShaderConverter Class
    - ![](https://img.shields.io/badge/new-green) ShaderObject Class
    - ![](https://img.shields.io/badge/new-green) ShaderType Enum

### Radiance v0.1.0

 - ![](https://img.shields.io/badge/new-green) Window Class
    - Open and Close Methods
    - Events
 - ![](https://img.shields.io/badge/new-green) Input Enum
 - ![](https://img.shields.io/badge/new-green) Vector Record
 - ![](https://img.shields.io/badge/new-green) Vertex Record
 - ![](https://img.shields.io/badge/new-green) Graphics Class
    - Clear Method
    - DrawPolygon Method
    - FillPolygon Method
