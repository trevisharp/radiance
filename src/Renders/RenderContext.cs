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
    
    public bool Verbose { get; set; } = false;
    
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

        var item = generateShaders(Position, Color, shaderCtx);

        start("Vertex Shader Creation");
        code(item.vertSrc);
        var vertexShader = createVertexShader(item.vertSrc);
        success("Shader Created!!");

        start("Fragment Shader Creation");
        code(item.fragSrc);
        var fragmentShader = createFragmentShader(item.fragSrc);
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

            if (item.vertStp is not null)
                item.vertStp();

            if (item.fragStp is not null)
                item.fragStp();

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

    private (string vertSrc, Action vertStp, 
        string fragSrc, Action fragStp) generateShaders(
        Vec3ShaderObject vertObj,
        Vec4ShaderObject fragObj,
        ShaderContext ctx
    )
    {
        var vertSb = getCodeBuilder();
        var fragSb = getCodeBuilder();

        Action vertStp = null;
        Action fragStp = null;

        var vertDeps = vertObj.Dependencies
            .Append(Utils.widthDep)
            .Append(Utils.heightDep)
            .Distinct();
        var fragDeps = fragObj.Dependencies
            .Distinct();

        var allDeps = vertDeps
            .Concat(fragDeps)
            .Distinct();

        foreach (var dep in vertDeps)
        {
            dep.AddHeader(vertSb);
            vertStp += dep.AddOperation(ctx);
        }

        foreach (var dep in fragDeps)
        {
            dep.AddHeader(fragSb);
            vertStp += dep.AddOperation(ctx);
        }
        
        foreach (var dep in allDeps)
        {
            dep.AddVertexHeader(vertSb);
            dep.AddFragmentHeader(fragSb);

            vertStp += dep.AddVertexOperation(ctx);
            fragStp += dep.AddFragmentOperation(ctx);
        }
        
        fragSb.AppendLine("out vec4 outColor;");
        initMain(vertSb);
        initMain(fragSb);

        foreach (var dep in vertDeps)
        {
            dep.AddCode(vertSb);
        }

        foreach (var dep in fragDeps)
        {
            dep.AddCode(fragSb);
        }
        
        foreach (var dep in allDeps)
        {
            dep.AddVertexCode(vertSb);
            dep.AddFragmentCode(fragSb);
        }

        vertSb.AppendLine($"\tvec3 finalPosition = {vertObj};");
        vertSb.AppendLine($"\tvec3 tposition = vec3(2 * finalPosition.x / width - 1, 2 * finalPosition.y / height - 1, finalPosition.z);");
        vertSb.AppendLine($"\tgl_Position = vec4(tposition, 1.0);");
        fragSb.AppendLine($"\toutColor = {fragObj};");
        closeMain(vertSb);
        closeMain(fragSb);

        return (vertSb.ToString(), vertStp, fragSb.ToString(), fragStp);

        void initMain(StringBuilder sb)
        {
            sb.AppendLine();
            sb.AppendLine("void main()");
            sb.AppendLine("{");
        }

        void closeMain(StringBuilder sb)
            => sb.Append('}');
        
        StringBuilder getCodeBuilder()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"#version {VersionText}");
            return sb;
        }
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