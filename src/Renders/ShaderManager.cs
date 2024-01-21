/* Author:  Leonardo Trevisan Silio
 * Date:    20/01/2024
 */
using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using static System.Console;

using OpenTK.Graphics.OpenGL4;

using StbImageSharp;

namespace Radiance.RenderFunctions;

using Data;
using Internal;

using ShaderSupport;
using ShaderSupport.Objects;
using ShaderSupport.Dependencies;

/// <summary>
/// A class to build GLSL shader code and manage OpenGL buffers.
/// </summary>
public class ShaderManager
{
    static Dictionary<int, int> shaderMap = new();
    static Dictionary<(int, int), int> programMap = new();
    static Dictionary<ImageResult, int> textureMap = new();
    
    private int globalTabIndex = 0;

    private event Action effects;
    private event Action unloadEffects;

    private List<int> bufferList = new();

    public bool Verbose { get; set; } = false;
    public string VersionText { get; set; } = "330 core";

    public ShaderManager(Action function)
    {
        discoverGlobalVariables(function);
    }
    public ShaderManager(Action<FloatShaderObject> function)
    {
        discoverGlobalVariables(function);
    }
    public ShaderManager(Action<
        FloatShaderObject, FloatShaderObject> function)
    {
        discoverGlobalVariables(function);
    }
    public ShaderManager(Action<
        FloatShaderObject, FloatShaderObject, 
        FloatShaderObject> function)
    {
        discoverGlobalVariables(function);
    }
    public ShaderManager(Action<
        FloatShaderObject, FloatShaderObject, 
        FloatShaderObject, FloatShaderObject> function)
    {
        discoverGlobalVariables(function);
    }

    public void Clear(Vec4 color)
    {
        effects += delegate
        {
            GL.ClearColor(
                color.X,
                color.Y,
                color.Z,
                color.W
            );
        };
    }

    public void FillTriangles(Polygon data)
        => baseDraw(PrimitiveType.Triangles, data);

    public void Draw(Polygon data) 
        => baseDraw(PrimitiveType.LineLoop, data);

    private void baseDraw(PrimitiveType type, Polygon data)
    {
        var ctx = RenderContext.GetContext();
        var vert = ctx.Position;
        var frag = ctx.Color;

        // TODO
        // var realOutputs = data.Outputs
        //     .Where(o => frag.Any(d => d.Name == o.BaseDependence.Name));
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

        var gpuBuffer = createBuffer();
        int vertexArray = createVertexArray(data);
        
        effects += delegate
        {
            GL.UseProgram(program);
            GL.BindVertexArray(vertexArray);

            GL.BindBuffer(
                BufferTarget.ArrayBuffer, 
                gpuBuffer
            );

            if (vertexTuple.setup is not null)
                vertexTuple.setup();

            if (fragmentTuple.setup is not null)
                fragmentTuple.setup();

            GL.DrawArrays(
                type,
                0, data.Elements
            );
        };

        unloadEffects += delegate
        {
            GL.DeleteVertexArray(vertexArray);
        };
    }

    internal void Render(Polygon polygon, float[] parameters)
    {
        if (effects is null)
            return;
        
        effects();
    }

    internal void Unload()
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

        unloadEffects();
    }

    private void discoverGlobalVariables(Delegate Function)
    {
        var mainType = Function.Method.DeclaringType;
        
        foreach (var field in mainType.GetRuntimeFields())
        {
            var type = field.FieldType;
            if (!type.IsSubclassOf(typeof(ShaderGlobalReference)))
                continue;
            
            var constructor = type.GetConstructor(
                new Type[] { 
                    typeof(FieldInfo), 
                    typeof(object),
                    typeof(object)
                }
            );

            var gref = field.GetValue(Function.Target)
                as ShaderGlobalReference;

            var obj = constructor.Invoke(
                new object[] {
                    field,
                    Function.Target,
                    gref.ObjectValue
                }
            );
            
            field.SetValue(Function.Target, obj);
        }
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

                    setup += delegate
                    {
                        float[] data = (float[])dependence.Value;

                        GL.BufferData(
                            BufferTarget.ArrayBuffer,
                            data.Length * sizeof(float), 
                            data, 
                            BufferUsageHint.DynamicDraw
                        );
                    };
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