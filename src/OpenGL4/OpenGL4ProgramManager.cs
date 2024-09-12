/* Author:  Leonardo Trevisan Silio
 * Date:    28/08/2024
 */
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL4;
using OpenTKShaderType = OpenTK.Graphics.OpenGL4.ShaderType;

namespace Radiance.OpenGL4;

using Managers;
using Primitives;
using Shaders.CodeGen;

/// <summary>
/// The manager for shaders and programs mapped to OpenGL4.
/// </summary>
public class OpenGL4ProgramManager : ProgramManager
{
    static readonly Dictionary<int, int> shaderMap = [];
    static readonly Dictionary<(int, int), int> programMap = [];

    /// <summary>
    /// Unload all OpenGL Resources.
    /// </summary>
    public override void FreeAllResources()
    {
        GL.UseProgram(0);
        foreach (var program in programMap)
            GL.DeleteProgram(program.Value);
        programMap.Clear();

        foreach (var shaderKey in shaderMap)
            GL.DeleteShader(shaderKey.Value);
        shaderMap.Clear();
    }
    public override int CreateProgram(
        ShaderPair pair,
        bool verbose = false
    )
    {
        int tabIndex = 0;
        var vertexShader = CreateVertexShader(pair.VertexShader, verbose, ref tabIndex);
        var fragmentShader = CreateFragmentShader(pair.FragmentShader, verbose, ref tabIndex);
        int program = CreateProgram(vertexShader, fragmentShader, verbose, ref tabIndex);
        return program;
    }

    public override void Clear(Vec4 color)
        => GL.ClearColor(color.X, color.Y, color.Z, color.W);

    public override void UseProgram(int program)
    {
        System.Console.WriteLine($"GL.UseProgram({program})");
        GL.UseProgram(program);
    }

    /// <summary>
    /// Compile a Vertex Shader and get his id.
    /// </summary>
    private static int CreateVertexShader(
        Shader shader,
        bool verbose,
        ref int tabIndex)
    {
        var shaderId = CreateShader(
            OpenTKShaderType.VertexShader,
            shader, verbose, ref tabIndex
        );
        return shaderId;
    }
    
    /// <summary>
    /// Compile a Fragment Shader and get his id.
    /// </summary>
    private static int CreateFragmentShader(
        Shader shader,
        bool verbose,
        ref int tabIndex)
    {
        var shaderId = CreateShader(
            OpenTKShaderType.FragmentShader,
            shader, verbose, ref tabIndex
        );
        return shaderId;
    }

    private static int CreateShader(
        OpenTKShaderType type,
        Shader shader,
        bool verbose,
        ref int tabIndex)
    {
        if (shaderMap.TryGetValue(shader.Hash, out int value))
            return value;
        
        Information("Cache miss. Create new shader!", verbose, ref tabIndex);
        
        Code(shader.Code, verbose, ref tabIndex);
        Information($"Hash: {shader.Hash}", verbose, ref tabIndex);

        var shaderId = GL.CreateShader(type);
        Information($"Code: {shaderId}", verbose, ref tabIndex);
        Information($"Compiling Shader...", verbose, ref tabIndex);
        
        GL.ShaderSource(shaderId, shader.Code);
        GL.CompileShader(shaderId);

        shaderMap.Add(shader.Hash, shaderId);

        GL.GetShader(shaderId, ShaderParameter.CompileStatus, out var code);
        if (code != (int)All.True)
        {
            var infoLog = GL.GetShaderInfoLog(shaderId);
            Error($"Error occurred in Shader({shaderId}) compilation: {infoLog}", verbose, ref tabIndex);
            return -1;
        }

        return shaderId;
    }

    /// <summary>
    /// Compile a Program from the Shader id's and his id.
    /// </summary>
    private static int CreateProgram(
        int vertexShader, 
        int fragmentShader,
        bool verbose,
        ref int tabIndex)
    {
        var programKey = (vertexShader, fragmentShader);
        if (programMap.TryGetValue(programKey, out int reusingProgram))
            return reusingProgram;

        Start("Creating Program...", verbose, ref tabIndex);
        var program = GL.CreateProgram();
        
        Information("Attaching Shaders...", verbose, ref tabIndex);
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        
        Information("Link Program...", verbose, ref tabIndex);
        GL.LinkProgram(program);

        Information("Dettaching Shaders...", verbose, ref tabIndex);
        GL.DetachShader(program, vertexShader);
        GL.DetachShader(program, fragmentShader);

        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
        if (code != (int)All.True)
            Error($"Error occurred Program({program}) linking.", verbose, ref tabIndex);
        
        programMap.Add(programKey, program);
        Success("Program Created!!", verbose, ref tabIndex);
        return program;
    }
}