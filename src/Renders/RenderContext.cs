/* Author:  Leonardo Trevisan Silio
 * Date:    25/01/2024
 */
using System;
using System.Text;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using static System.Console;

using OpenTK.Graphics.OpenGL4;

namespace Radiance.Renders;

using Data;
using Shaders;
using Shaders.Objects;

/// <summary>
/// A global thread-safe context to shader construction.
/// </summary>
public class RenderContext
{
    private static Dictionary<int, RenderContext> threadMap = new();
    internal static RenderContext CreateContext()
    {
        var crr = Thread.CurrentThread;
        var id  = crr.ManagedThreadId;
        if (threadMap.ContainsKey(id))
            threadMap.Remove(id);
    
        var ctx = new RenderContext();
        threadMap.Add(id, ctx);
        return ctx;
    }
    internal static RenderContext GetContext()
    {
        var crr = Thread.CurrentThread;
        var id  = crr.ManagedThreadId;
        if (threadMap.ContainsKey(id))
            return threadMap[id];
        
        return null;
    }

    static Dictionary<int, int> shaderMap = new();
    static Dictionary<(int, int), int> programMap = new();

    /// <summary>
    /// Unload all OpenGL Resources.
    /// </summary>
    public static void FreeAllResources()
    {
        GL.UseProgram(0);
        foreach (var program in programMap)
            GL.DeleteProgram(program.Value);
        programMap.Clear();

        foreach (var shaderKey in shaderMap)
            GL.DeleteShader(shaderKey.Value);
        shaderMap.Clear();
    }
    
    public static bool Verbose { get; set; } = false;
    
    private int globalTabIndex = 0;
    private event Action<Polygon, object[]> operations;

    public string VersionText { get; set; } = "330 core";
    public Vec3ShaderObject Position { get; set; }
    public Vec4ShaderObject Color { get; set; }
    
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

    private void baseDraw(bool isFill)
    {
        start("Creating Program");
        ShaderContext shaderCtx = new ShaderContext();

        foreach (var dep in Position.Dependencies)
            information(dep.ToString());
        System.Console.WriteLine();
        foreach (var dep in Color.Dependencies)
            information(dep.ToString());

        start("Vertex Shader Creation");
        var vertexTuple = generateVertexShader(Position, Color, shaderCtx);
        var vertexShader = createVertexShader(vertexTuple.source);
        success("Shader Created!!");

        start("Fragment Shader Creation");
        var fragmentTuple = generateFragmentShader(Position, Color, shaderCtx);
        var fragmentShader = createFragmentShader(fragmentTuple.source);
        success("Shader Created!!");

        int program = createProgram(vertexShader, fragmentShader);
        shaderCtx.Program = program;
        success("Program Created!!");
        
        operations += (poly, data) =>
        {
            if (isFill)
                poly = poly.Triangulation;

            shaderCtx.CreateResources(poly);
            GL.UseProgram(program);

            shaderCtx.Use(poly);

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

        var hash = source.GetHashCode();
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

    private (string source, Action setup) generateVertexShader(
        Vec3ShaderObject vertexObject,
        Vec4ShaderObject fragmentObject,
        ShaderContext ctx
    )
    {
        information($"Generating Shader...");
        var sb = getCodeBuilder();
        Action setup = null;

        var deps = vertexObject.Dependencies
            .Append(Utils.widthDep)
            .Append(Utils.heightDep);
        
        foreach (var dep in deps)
        {
            dep.AddVertexHeader(sb);
            dep.AddHeader(sb);

            setup += dep.AddVertexOperation(ctx);
            setup += dep.AddOperation(ctx);
        }

        foreach (var dep in fragmentObject.Dependencies)
        {
            dep.AddVertexHeader(sb);
            setup += dep.AddVertexOperation(ctx);
        }

        sb.AppendLine();
        sb.AppendLine("void main()");
        sb.AppendLine("{");
        foreach (var dep in deps)
        {
            dep.AddCode(sb);
            dep.AddVertexCode(sb);
        }

        foreach (var dep in fragmentObject.Dependencies)
        {
            dep.AddVertexCode(sb);
        }

        sb.AppendLine($"\tvec3 finalPosition = {vertexObject.Expression};");
        sb.AppendLine($"\tvec3 tposition = vec3(2 * finalPosition.x / width - 1, 2 * finalPosition.y / height - 1, finalPosition.z);");
        sb.AppendLine($"\tgl_Position = vec4(tposition, 1.0);");
        sb.Append("}");

        var result = sb.ToString();

        information("Vertex Shader:");
        code(result);

        return (result, setup);
    }

    private (string source, Action setup) generateFragmentShader(
        Vec3ShaderObject vertexObject,
        Vec4ShaderObject fragmentObject,
        ShaderContext ctx
    )
    {
        information($"Generating Shader...");

        var sb = getCodeBuilder();
        Action setup = null;
        
        var deps = fragmentObject.Dependencies;
        
        foreach (var dep in deps)
        {
            dep.AddFragmentHeader(sb);
            dep.AddHeader(sb);

            setup += dep.AddFragmentOperation(ctx);
            setup += dep.AddOperation(ctx);
        }

        foreach (var dep in vertexObject.Dependencies)
        {
            dep.AddFragmentHeader(sb);
            setup += dep.AddFragmentOperation(ctx);
        }

        sb.AppendLine();
        sb.AppendLine("out vec4 outColor;");
        sb.AppendLine("void main()");
        sb.AppendLine("{");
        foreach (var dep in deps)
        {
            dep.AddCode(sb);
            dep.AddFragmentCode(sb);
        }

        foreach (var dep in vertexObject.Dependencies)
        {
            dep.AddFragmentCode(sb);
        }

        
        sb.AppendLine($"\toutColor = {fragmentObject.Expression};");
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