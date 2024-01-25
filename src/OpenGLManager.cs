/* Author:  Leonardo Trevisan Silio
 * Date:    22/01/2024
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
    
    public static bool Verbose { get; set; } = false;
    
    private int globalTabIndex = 0;
    private event Action<Polygon, object[]> operations;

    public string VersionText { get; set; } = "330 core";
    
    public void Render(Polygon polygon, object[] parameters)
    {
        if (operations is null)
            return;
        
        operations(polygon, parameters);
    }

    public void AddClear(Vec4 color)
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

    public void AddFill()
        => baseDraw(true);

    public void AddDraw() 
        => baseDraw(false);

    private void createResources(Polygon poly)
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

        start("Creating Program");
        var programData = new int[] { 0 };

        start("Vertex Shader Creation");
        var vertexTuple = generateVertexShader(
            vert, programData
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

            createResources(poly);
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
        int[] programData
    )
    {
        information($"Generating Shader...");
        var sb = getCodeBuilder();
        Action setup = null;

        var dependencens = vertexObject.Dependencies
            .Append(Utils._width)
            .Append(Utils._height)
            .Distinct(ShaderDependence.Comparer);
        
        var codeDeps = vertexObject.Dependecies
            .Where(dep => dep is CodeDependence)
            .Select(dep => dep as CodeDependence);

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
                    sb.AppendLine(dependence.GetHeader());

                    setup += delegate
                    {
                        setTextureData(programData[0], dependence);
                    };
                    break;
                
                case ShaderDependenceType.CustomData:
                    sb.AppendLine(dependence.GetHeader());
                    break;
            }
        }

        var exp = vertexObject.Expression;

        sb.AppendLine();
        sb.AppendLine("void main()");
        sb.AppendLine("{");
        foreach (var codeDep in codeDeps)
            sb.AppendLine("\t" + codeDep.GetCode() + ";");

        sb.AppendLine($"\tvec3 finalPosition = {exp};");
        sb.AppendLine($"\tvec3 tposition = vec3(2 * finalPosition.x / width - 1, 2 * finalPosition.y / height - 1, finalPosition.z);");
        sb.AppendLine($"\tgl_Position = vec4(tposition, 1.0);");

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
        
        var dependencens = fragmentObject.Dependecies
            .Distinct(ShaderDependence.Comparer);
        
        var codeDeps = fragmentObject.Dependecies
            .Where(dep => dep is CodeDependence)
            .Select(dep => dep as CodeDependence);
        
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
                    sb.AppendLine(dependence.GetHeader());

                    setup += delegate
                    {
                        setTextureData(programData[0], dependence);
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
        foreach (var codeDep in codeDeps)
            sb.AppendLine("\t" + codeDep.GetCode() + ";");
        
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