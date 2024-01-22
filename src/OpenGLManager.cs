/* Author:  Leonardo Trevisan Silio
 * Date:    21/01/2024
 */
using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using static System.Console;

using StbImageSharp;
using OpenTK.Graphics.OpenGL4;

namespace Radiance;

using Data;
using Internal;

using Shaders;
using Shaders.Objects;
using Shaders.Dependencies;

/// <summary>
/// A class to manage OpenGL buffers, shaders and programs.
/// </summary>
public class OpenGLManager
{
    // Global OpenGL resources indexes map
    static Dictionary<int, int> shaderMap = new();
    static Dictionary<(int, int), int> programMap = new();
    static Dictionary<ImageResult, int> textureMap = new();
    static List<int> bufferList = new();
    static List<int> vertexArrayList = new();
    
    public static bool Verbose { get; set; } = false;

    public static void FreeAllResources()
    {
        GL.UseProgram(0);
        foreach (var program in programMap)
            GL.DeleteProgram(program.Value);
        programMap.Clear();

        foreach (var buffer in bufferList)
            GL.DeleteBuffer(buffer);
        bufferList.Clear();

        foreach (var shaderKey in shaderMap)
            GL.DeleteShader(shaderKey.Value);
        shaderMap.Clear();

        foreach (var vertexArray in vertexArrayList)
            GL.DeleteVertexArray(vertexArray);
    }
    
    private int globalTabIndex = 0;

    private event Action<Polygon, float[]> operations;
    private event Action unloadEffects;

    public string VersionText { get; set; } = "330 core";

    public void Render(Polygon polygon, float[] parameters)
    {
        if (operations is null)
            return;
        
        operations(polygon, parameters);
    }

    public void Clear(Vec4 color)
    {
        operations += delegate
        {
            GL.ClearColor(
                color.X,
                color.Y,
                color.Z,
                color.W
            );
        };
    }

    public void Fill()
    {
        baseDraw(true);
    }

    public void Draw() 
    {
        baseDraw(false);
    }

    /// <summary>
    /// Create Resources of OpenGL based on poly state and changes.
    /// </summary>
    public void CreateResources(Polygon poly)
    {
        if (poly.VertexObjectArray > -1 && poly.Buffer > -1)
            return;

        updateResources(poly, true, true);
        poly.OnChange += (bufferBreak, layoutBreak) =>
            updateResources(poly, bufferBreak, layoutBreak);
    }

    private void bindBuffer(Polygon poly)
    {
        GL.BindBuffer(
            BufferTarget.ArrayBuffer, 
            poly.Buffer
        );
    }

    private void bindVertexArray(Polygon poly)
    {
        GL.BindVertexArray(
            poly.VertexObjectArray
        );
    }

    private void updateResources(Polygon poly, bool bufferBreak, bool layoutBreak)
    {
        if (bufferBreak)
        {
            if (poly.Buffer > -1)
                GL.DeleteBuffer(poly.Buffer);

            int buffer = createBuffer();
            poly.Buffer = buffer;
        }
        bindBuffer(poly);

        if (layoutBreak)
        {
            if (poly.VertexObjectArray > -1)
                GL.DeleteVertexArray(poly.VertexObjectArray);

            int vertexArray = createVertexArray(poly);
            poly.VertexObjectArray = vertexArray;
        }
        bindVertexArray(poly);

        var data = poly.Data;
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            data.Length * sizeof(float), data, 
            BufferUsageHint.DynamicDraw
        );
    }

    private void baseDraw(bool isFill)
    {
        var ctx = RenderContext.GetContext();
        var vert = ctx.Position;
        var frag = ctx.Color;

        // TODO
        var realOutputs = new ShaderOutput[0];

        start("Creating Program");
        var programData = new int[] { 0 };

        start("Vertex Shader Creation");
        var vertexTuple = generateVertexShader(
            vert, realOutputs, programData
        );
        var vertexShader = createVertexShader(vertexTuple.source);
        success("Shader Created!!");

        start("Fragment Shader Creation");
        var fragmentTuple = generateFragmentShader(
            frag, programData
        );
        var fragmentShader = createFragmentShader(fragmentTuple.source);
        success("Shader Created!!");

        int program = createProgram(vertexShader, fragmentShader);
        programData[0] = program;
        success("Program Created!!");
        
        operations += (poly, data) =>
        {
            if (isFill)
                poly = poly.Triangulation;

            CreateResources(poly);
            GL.UseProgram(program);
            
            bindVertexArray(poly);
            bindBuffer(poly);

            if (vertexTuple.setup is not null)
                vertexTuple.setup();

            if (fragmentTuple.setup is not null)
                fragmentTuple.setup();

            GL.DrawArrays(
                isFill ? PrimitiveType.Triangles : PrimitiveType.LineLoop, 
                0, poly.Elements
            );
        };
    }
    
    private int createVertexShader(string source)
    {
        return createShader(
            OpenTK.Graphics.OpenGL4.ShaderType.VertexShader,
            source
        );
    }
    
    private int createFragmentShader(string source)
    {
        return createShader(
            OpenTK.Graphics.OpenGL4.ShaderType.FragmentShader,
            source
        );
    }

    private int createShader(OpenTK.Graphics.OpenGL4.ShaderType type, string source)
    {
        information("Creating Shader...");

        var hash = getHash(source);
        information($"Hash: {hash}");

        if (shaderMap.ContainsKey(hash))
        {
            information("Conflit. Reusing other shader!");
            return shaderMap[hash];
        }

        var shader = GL.CreateShader(type);
        information($"Code: {shader}");
        information($"Compiling Shader...");
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);

        shaderMap.Add(hash, shader);

        GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
        if (code != (int)All.True)
        {
            var infoLog = GL.GetShaderInfoLog(shader);
            error($"Error occurred in Shader({shader}) compilation: {infoLog}");
            return -1;
        }

        return shader;
    }

    private int createProgram(
        int vertexShader, 
        int fragmentShader
    )
    {
        var programKey = (vertexShader, fragmentShader);
        if (programMap.ContainsKey(programKey))
        {
            var reusingProgram = programMap[programKey];
            information($"Reusing Program {reusingProgram}.");
            return reusingProgram;
        }

        var program = GL.CreateProgram();
        
        information("Attaching Shaders...");
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        
        information("Link Program...");
        GL.LinkProgram(program);

        information("Dettaching Program...");
        GL.DetachShader(program, vertexShader);
        GL.DetachShader(program, fragmentShader);

        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
        if (code != (int)All.True)
            error($"Error occurred Program({program}) linking.");
        
        programMap.Add(programKey, program);
        return program;
    }

    private int getHash(string str)
        => str.GetHashCode();

    private int createBuffer()
    {
        var bufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, bufferObject);
        bufferList.Add(bufferObject);
        return bufferObject;
    }

    private List<ShaderDependence> getDependences(IEnumerable<ShaderObject> objs)
    {
        var dependences = new List<ShaderDependence>();
        foreach (var obj in objs)
        {
            foreach (var dependence in obj.Dependecies)
            {
                if (dependences.Contains(dependence))
                    continue;
                
                dependences.Add(dependence);
            }
        }
        return dependences;
    }

    private (string source, Action setup) generateVertexShader(
        Vec3ShaderObject vertexObject,
        IEnumerable<ShaderOutput> outputs,
        int[] programData
    )
    {
        information($"Generating Shader...");
        var sb = getCodeBuilder();
        Action setup = null;

        var outDeps = outputs
            .SelectMany(o => o.BaseValue.Dependecies);

        int textureId = 0;
        var dependencens = vertexObject.Dependecies
            .Append(Utils._width)
            .Append(Utils._height)
            .Concat(outDeps)
            .Distinct(ShaderDependence.Comparer);
        foreach (var dependence in dependencens)
        {
            switch (dependence.DependenceType)
            {
                case ShaderDependenceType.Uniform:
                    sb.AppendLine(dependence.GetHeader());

                    setup += delegate
                    {
                        setUniform(programData[0], dependence);
                    };
                    break;
                
                case ShaderDependenceType.Texture:
                    int id = textureId;
                    textureId++;

                    sb.AppendLine($"uniform sampler2D texture{id};");

                    setup += delegate
                    {
                        setTextureData(programData[0], id, dependence);
                    };
                    break;
                
                case ShaderDependenceType.CustomData:
                    sb.AppendLine(dependence.GetHeader());
                    break;
            }
        }

        foreach (var output in outputs)
        {
            var type = output.BaseValue.Type;
            var name = output.BaseDependence.Name;
            sb.AppendLine($"out {getShaderTypeName(type)} {name};");
        }
        
        var exp = vertexObject.Expression;

        sb.AppendLine();
        sb.AppendLine("void main()");
        sb.AppendLine("{");
        sb.AppendLine($"\tvec3 finalPosition = {exp};");
        sb.AppendLine($"\tvec3 tposition = vec3(2 * finalPosition.x / width - 1, 2 * finalPosition.y / height - 1, finalPosition.z);");
        sb.AppendLine($"\tgl_Position = vec4(tposition, 1.0);");

        foreach (var output in outputs)
        {
            var outExp = output.BaseValue.Expression;
            var name = output.BaseDependence.Name;
            sb.AppendLine($"\t{name} = {outExp};");
        }

        sb.Append("}");

        var result = sb.ToString();

        information("Vertex Shader:");
        code(result);

        return (result, setup);
    }

    private (string source, Action setup) generateFragmentShader(
        Vec4ShaderObject fragmentObject,
        int[] programData
    )
    {
        information($"Generating Shader...");

        var sb = getCodeBuilder();
        Action setup = null;
        
        int textureId = 0;
        var dependencens = fragmentObject.Dependecies
            .Distinct(ShaderDependence.Comparer);
        foreach (var dependence in dependencens)
        {
            switch (dependence.DependenceType)
            {
                case ShaderDependenceType.Uniform:
                    sb.AppendLine(dependence.GetHeader());

                    setup += delegate
                    {
                        setUniform(programData[0], dependence);
                    };
                    break;
                
                case ShaderDependenceType.Texture:
                    int id = textureId;
                    textureId++;

                    sb.AppendLine($"uniform sampler2D texture{id};");

                    setup += delegate
                    {
                        setTextureData(programData[0], id, dependence);
                    };
                    break;
                
                case ShaderDependenceType.Variable:
                    sb.AppendLine(dependence.GetHeader());
                    break;
            }
        }
        
        var exp = fragmentObject.Expression;

        sb.AppendLine();
        sb.AppendLine("out vec4 outColor;");
        sb.AppendLine("void main()");
        sb.AppendLine("{");
        sb.AppendLine($"\toutColor = {exp};");
        sb.Append("}");

        var result = sb.ToString();

        information("Fragment Shader:");
        code(result);

        return (result, setup);
    }

    private StringBuilder getCodeBuilder()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"#version {VersionText}");
        return sb;
    }
    
    private string getShaderTypeName(ShaderType type)
        => type switch
        {
            ShaderType.Float => "float",
            ShaderType.Vec2 => "vec2",
            ShaderType.Vec3 => "vec3",
            ShaderType.Vec4 => "vec4",
            ShaderType.Bool => "bool",
            _ => "float"
        };

    private void setUniform(int program, ShaderDependence dependence)
    {
        switch (dependence)
        {
            case ShaderDependence<FloatShaderObject>:
                setUniformFloat(program, dependence.Name, (float)dependence.Value);
                break;
        }
    }

    private void setUniformFloat(int program, string name, float value)
    {
        var code = GL.GetUniformLocation(program, name);
        GL.Uniform1(code, value);
    }

    private void setTextureData(int program, int id, ShaderDependence dependence)
    {
        var texture = dependence as TextureDependence;
        if (texture is null)
            return;

        activateImage(texture.Image, id);
        var code = GL.GetUniformLocation(program, $"texture{id}");
        GL.Uniform1(code, id);
    }

    private void activateImage(ImageResult image, int id)
    {
        int handle = getTextureHandle(image);
        GL.ActiveTexture(TextureUnit.Texture0 + id);
        GL.BindTexture(TextureTarget.Texture2D, handle);
    }

    private int getTextureHandle(ImageResult image)
    {
        if (textureMap.ContainsKey(image))
            return textureMap[image];
        
        int handle = initImageData(image);
        textureMap.Add(image, handle);
        return handle;
    }

    private int initImageData(ImageResult image)
    {
        int handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, handle);
        
        GL.TexImage2D(
            TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
            image.Width, image.Height, 0, PixelFormat.Rgba,
            PixelType.UnsignedByte, image.Data
        );   
        GL.TexParameter(
            TextureTarget.Texture2D, 
            TextureParameterName.TextureWrapS, 
            (int)TextureWrapMode.Repeat
        );
        GL.TexParameter(
            TextureTarget.Texture2D,
            TextureParameterName.TextureWrapT,
            (int)TextureWrapMode.Repeat
        );
        GL.TexParameter(
            TextureTarget.Texture2D,
            TextureParameterName.TextureMagFilter,
            (int)TextureMagFilter.Linear
        );
        GL.TexParameter(
            TextureTarget.Texture2D,
            TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.Linear
        );
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        return handle;
    }

    private int createVertexArray(Polygon data)
    {
        int vertexObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexObject);

        int total = data.Layouts.Sum(layout => layout.size);
        var stride = total * sizeof(float);
        var type = VertexAttribPointerType.Float;

        int i = 0;
        int offset = 0;
        foreach (var layout in data.Layouts)
        {
            GL.VertexAttribPointer(i, layout.size, type, false, stride, offset);
            GL.EnableVertexAttribArray(i);
            offset += layout.size * sizeof(float);
            i++;
        }

        vertexArrayList.Add(vertexObject);
        return vertexObject;
    }

    private void error(string message = "")
        => verbose(message, ConsoleColor.White, ConsoleColor.Red, globalTabIndex);
    
    private void information(string message = "")
        => verbose(message, ConsoleColor.Green, ConsoleColor.Black, globalTabIndex);
    
    private void success(string message = "")
        => verbose(message + "\n", ConsoleColor.Blue, ConsoleColor.Black, --globalTabIndex);
    
    private void code(string message = "")
        => verbose(message, ConsoleColor.DarkYellow, ConsoleColor.Black, globalTabIndex + 1);

    private void start(string message = "")
        => verbose("Start: " + message, ConsoleColor.Magenta, ConsoleColor.Black, globalTabIndex++);

    private void verbose(
        string text, 
        ConsoleColor fore = ConsoleColor.White,
        ConsoleColor back = ConsoleColor.Black,
        int tabIndex = 0,
        bool newline = true
        )
    {
        if (!Verbose)
            return;
        
        var fullTab = "";
        for (int i = 0; i < tabIndex; i++)
            fullTab += "\t";

        text = fullTab + text.Replace("\n", "\n" + fullTab);
        
        ForegroundColor = fore;
        BackgroundColor = back;
        Write(text);
        
        if (newline)
            WriteLine();
    }
}