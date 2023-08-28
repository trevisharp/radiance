/* Author:  Leonardo Trevisan Silio
 * Date:    23/08/2023
 */
using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using static System.Console;

using OpenTK.Graphics.OpenGL4;

namespace Radiance.RenderFunctions;

using Data;

using ShaderSupport;
using ShaderSupport.Objects;

/// <summary>
/// Provide render operations to draw data in screen.
/// </summary>
public class RenderOperations
{
    public bool Verbose { get; set; } = false;
    public string VersionText { get; set; } = "330 core";

    public RenderOperations(Delegate Function)
    {
        discoverGlobalVariables(Function);
        effects += delegate
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        };
    }

    public void Clear(Color color)
    {
        effects += delegate
        {
            GL.ClearColor(
                color.R,
                color.G,
                color.B,
                color.A
            );
        };
    }

    public void FillTriangles(
        IData data
    ) => baseDraw(PrimitiveType.Triangles, data);
        
    public void Draw(
        IData data
    ) => baseDraw(PrimitiveType.LineLoop, data);

    private void baseDraw(
        PrimitiveType type,
        IData data
    )
    {
        var gpuBuffer = createBuffer();
        int vertexArray = createVertexArray(data);
        int program = createProgram();

        var frag = data.FragmentObject.Dependecies;
        var realOutputs = data.Outputs
            .Where(o => frag.Any(d => d.Name == o.BaseDependence.Name));

        start("Vertex Shader Creation");
        var vertexTuple = generateVertexShader(
            data.VertexObject, realOutputs, program
        );
        var verteShader = createVertexShader(vertexTuple.source);

        start("Fragment Shader Creation");
        var fragmentTuple = generateFragmentShader(
            data.FragmentObject, program
        );
        var fragmentShader = createFragmentShader(fragmentTuple.source);

        addShaders(
            program,
            verteShader,
            fragmentShader
        );
        
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

    internal void Render(params object[] parameters)
    {
        if (effects is null)
            return;
        
        effects(parameters);
    }

    internal void Unload()
    {
        GL.UseProgram(0);
        foreach (var program in programList)
            GL.DeleteProgram(program);
        programList.Clear();

        foreach (var buffer in bufferList)
            GL.DeleteBuffer(buffer);
        bufferList.Clear();

        foreach (var shaderKey in shaderMap)
            GL.DeleteShader(shaderKey.Value);
        shaderMap.Clear();

        unloadEffects();
    }

    private event Action<object[]> effects;
    private event Action unloadEffects;

    private List<int> bufferList = new();

    private List<int> programList = new();

    private Dictionary<int, int> shaderMap = new();

    private void discoverGlobalVariables(Delegate Function)
    {
        var mainType = Function.Method.DeclaringType;
        
        foreach (var field in mainType.GetRuntimeFields())
        {
            var type = field.FieldType;
            if (!type.IsSubclassOf(typeof(ShaderReference)))
                continue;
            
            var constructor = type.GetConstructor(
                new Type[] { 
                    typeof(FieldInfo), 
                    typeof(object) 
                }
            );

            var obj = constructor.Invoke(
                new object[] {
                    field,
                    Function.Target
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

        success("Shader Created!!");
        return shader;
    }

    private int createProgram()
    {
        var program = GL.CreateProgram();
        this.programList.Add(program);
        return program;
    }

    private int getHash(string str)
        => str.GetHashCode();

    private void addShaders(int program, int vertexShader, int fragmentShader)
    {
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        
        GL.LinkProgram(program);

        GL.DetachShader(program, vertexShader);
        GL.DetachShader(program, fragmentShader);

        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
        if (code != (int)All.True)
            error($"Error occurred Program({program}) linking.");
    }
    
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
        int program)
    {
        information($"Generating Shader...");
        var sb = getCodeBuilder();
        Action setup = null;

        var outDeps = outputs
            .SelectMany(o => o.BaseValue.Dependecies);

        var dependencens = vertexObject.Dependecies
            .Append(RadianceUtils._width)
            .Append(RadianceUtils._height)
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
                        setUniform(program, dependence);
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
        int program)
    {
        information($"Generating Shader...");

        var sb = getCodeBuilder();
        Action setup = null;

        var dependencens = fragmentObject.Dependecies
            .Distinct();
        foreach (var dependence in dependencens)
        {
            switch (dependence.DependenceType)
            {
                case ShaderDependenceType.Uniform:
                    sb.AppendLine(dependence.GetHeader());

                    setup += delegate
                    {
                        setUniform(program, dependence);
                    };
                    break;
                
                case ShaderDependenceType.Variable:
                    sb.Append(dependence.GetHeader());
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

    private int createVertexArray(IData data)
    {
        int vertexObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexObject);

        int total = data.Sizes.Sum();
        var stride = total * sizeof(float);
        var type = VertexAttribPointerType.Float;

        int i = 0;
        int offset = 0;
        foreach (var size in data.Sizes)
        {
            GL.VertexAttribPointer(i, size, type, false, stride, offset);
            GL.EnableVertexAttribArray(i);
            offset += size * sizeof(float);
            i++;
        }

        return vertexObject;
    }

    private void error(string message = "")
        => verbose(message, ConsoleColor.White, ConsoleColor.Red);
    
    private void information(string message = "")
        => verbose(message, ConsoleColor.Green);
    
    private void success(string message = "")
        => verbose(message + "\n", ConsoleColor.Blue);
    
    private void code(string message = "")
        => verbose(message, ConsoleColor.DarkYellow, ConsoleColor.Black, 1);

    private void start(string message = "")
        => verbose("Process: " + message, ConsoleColor.Magenta);

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