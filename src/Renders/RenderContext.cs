/* Author:  Leonardo Trevisan Silio
 * Date:    19/08/2024
 */
using System;
using System.Text;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using static System.Console;

using OpenTK.Graphics.OpenGL4;
using OpenTKShaderType = OpenTK.Graphics.OpenGL4.ShaderType;

namespace Radiance.Renders;

using Data;
using Shaders;
using Shaders.Objects;

/// <summary>
/// A global thread-safe context to shader construction.
/// </summary>
public class RenderContext
{
    private static readonly Dictionary<int, RenderContext> threadMap = [];

    internal static RenderContext CreateContext()
    {
        var id = GetCurrentThreadId();
        threadMap.Remove(id);

        var ctx = new RenderContext
        {
            Position = new("pos", ShaderOrigin.VertexShader, [ Utils.bufferDep ]),
            Color = new("vec4(0.0, 0.0, 0.0, 1.0)", ShaderOrigin.FragmentShader, [])
        };
        threadMap.Add(id, ctx);
        return ctx;
    }

    internal static void ClearContext()
    {
        var id = GetCurrentThreadId();
        threadMap.Remove(id);
    }

    internal static RenderContext GetContext()
    {
        var id = GetCurrentThreadId();
        return threadMap.TryGetValue(id, out RenderContext value) ? value : null;
    }

    internal static int GetCurrentThreadId()
    {
        var crr = Thread.CurrentThread;
        var id  = crr.ManagedThreadId;
        return id;
    }

    static readonly Dictionary<int, int> shaderMap = [];
    static readonly Dictionary<(int, int), int> programMap = [];

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
    
    public bool IsVerbose { get; set; } = false;

    private int globalTabIndex = 0;
    private event Action<Polygon, object[]> Pipeline;

    public string VersionText { get; set; } = "330 core";
    public Vec3ShaderObject Position { get; set; }
    public Vec4ShaderObject Color { get; set; }
    
    public void Render(Polygon polygon, object[] parameters)
    {
        if (Pipeline is null)
            return;
        
        Pipeline(polygon, parameters);
    }

    public void AddClear(Vec4 color)
    {
        Pipeline += delegate
        {
            GL.ClearColor(
                color.X,
                color.Y,
                color.Z,
                color.W
            );
        };
    }

    public void AddPoints() 
        => AddDrawOperation(PrimitiveType.Points);

    public void AddLines() 
        => AddDrawOperation(PrimitiveType.Lines);
    
    public void AddDraw() 
        => AddDrawOperation(PrimitiveType.LineLoop);
    
    public void AddFill()
        => AddDrawOperation(PrimitiveType.Triangles, true);
    
    public void AddTriangules() 
        => AddDrawOperation(PrimitiveType.Triangles);
    
    public void AddStrip() 
        => AddDrawOperation(PrimitiveType.TriangleStrip);
    
    public void AddFan() 
        => AddDrawOperation(PrimitiveType.TriangleFan);

    private void AddDrawOperation(
        PrimitiveType primitive, 
        bool needTriangularization = false
    )
    {
        Start("Creating Program");
        ShaderContext shaderCtx = new ShaderContext();

        var (vertSource, vertSetup, fragSoruce, fragSetup) = GenerateShaders(Position, Color, shaderCtx);

        Start("Vertex Shader Creation");
        Code(vertSource);
        var vertexShader = CreateVertexShader(vertSource);
        Success("Shader Created!!");

        Start("Fragment Shader Creation");
        Code(fragSoruce);
        var fragmentShader = CreateFragmentShader(fragSoruce);
        Success("Shader Created!!");

        int program = CreateProgram(vertexShader, fragmentShader);
        shaderCtx.Program = program;
        Success("Program Created!!");
        
        Pipeline += (poly, data) =>
        {
            if (needTriangularization)
                poly = poly.Triangulation;

            shaderCtx.CreateResources(poly);
            GL.UseProgram(program);

            shaderCtx.Use(poly);

            if (vertSetup is not null)
                vertSetup();

            if (fragSetup is not null)
                fragSetup();

            GL.DrawArrays(primitive, 0, poly.Data.Count() / 3);
        };
    }
    
    private int CreateVertexShader(string source)
    {
        return CreateShader(
            OpenTKShaderType.VertexShader,
            source
        );
    }
    
    private int CreateFragmentShader(string source)
    {
        return CreateShader(
            OpenTKShaderType.FragmentShader,
            source
        );
    }

    private int CreateShader(OpenTKShaderType type, string source)
    {
        Information("Getting Shader...");

        var hash = source.GetHashCode();
        Information($"Hash: {hash}");

        if (shaderMap.TryGetValue(hash, out int value))
        {
            Information("Reusing other shader!");
            return value;
        }
        Information("Cache miss. Create new shader!");

        var shader = GL.CreateShader(type);
        Information($"Code: {shader}");
        Information($"Compiling Shader...");
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);

        shaderMap.Add(hash, shader);

        GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
        if (code != (int)All.True)
        {
            var infoLog = GL.GetShaderInfoLog(shader);
            Error($"Error occurred in Shader({shader}) compilation: {infoLog}");
            return -1;
        }

        return shader;
    }

    private int CreateProgram(
        int vertexShader, 
        int fragmentShader
    )
    {
        var programKey = (vertexShader, fragmentShader);
        if (programMap.ContainsKey(programKey))
        {
            var reusingProgram = programMap[programKey];
            Information($"Reusing Program {reusingProgram}.");
            return reusingProgram;
        }

        var program = GL.CreateProgram();
        
        Information("Attaching Shaders...");
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        
        Information("Link Program...");
        GL.LinkProgram(program);

        Information("Dettaching Program...");
        GL.DetachShader(program, vertexShader);
        GL.DetachShader(program, fragmentShader);

        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
        if (code != (int)All.True)
            Error($"Error occurred Program({program}) linking.");
        
        programMap.Add(programKey, program);
        return program;
    }

    private (string vertSrc, Action vertStp, string fragSrc, Action fragStp) GenerateShaders(
        Vec3ShaderObject vertObj, Vec4ShaderObject fragObj, ShaderContext ctx
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
            dep.AddCode(vertSb);

        foreach (var dep in fragDeps)
            dep.AddCode(fragSb);
        
        foreach (var dep in allDeps)
        {
            dep.AddVertexCode(vertSb);
            dep.AddFragmentCode(fragSb);
        }

        vertSb.AppendLine($"\tvec3 finalPosition = {vertObj};");
        vertSb.AppendLine($"\tvec3 tposition = vec3(2 * finalPosition.x / width - 1, 2 * finalPosition.y / height - 1, finalPosition.z);");
        vertSb.AppendLine($"\tgl_Position = vec4(tposition, 1.0);");
        fragSb.AppendLine($"\toutColor = {fragObj};");

        foreach (var dep in allDeps)
        {
            dep.AddVertexFinalCode(vertSb);
            dep.AddFragmentFinalCode(fragSb);
        }

        foreach (var dep in vertDeps)
            dep.AddFinalCode(vertSb);
        
        foreach (var dep in fragDeps)
            dep.AddFinalCode(fragSb);

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

    private void Error(string message = "")
        => Verbose(message, ConsoleColor.White, ConsoleColor.Red, globalTabIndex);
    
    private void Information(string message = "")
        => Verbose(message, ConsoleColor.Green, ConsoleColor.Black, globalTabIndex);
    
    private void Success(string message = "")
        => Verbose(message + "\n", ConsoleColor.Blue, ConsoleColor.Black, --globalTabIndex);
    
    private void Code(string message = "")
        => Verbose(message, ConsoleColor.DarkYellow, ConsoleColor.Black, globalTabIndex + 1);

    private void Start(string message = "")
        => Verbose("Start: " + message, ConsoleColor.Magenta, ConsoleColor.Black, globalTabIndex++);

    private void Verbose(
        string text, 
        ConsoleColor fore = ConsoleColor.White,
        ConsoleColor back = ConsoleColor.Black,
        int tabIndex = 0,
        bool newline = true
    )
    {
        if (!IsVerbose)
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