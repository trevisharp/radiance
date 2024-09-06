/* Author:  Leonardo Trevisan Silio
 * Date:    28/08/2024
 */
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL4;
using OpenTKShaderType = OpenTK.Graphics.OpenGL4.ShaderType;

namespace Radiance.Contexts.OpenGL4;

using Radiance.Contexts;
using Radiance.Primitives;

/// <summary>
/// The manager for shaders and programs mapped to OpenGL4.
/// </summary>
public class OpenGL4ProgramContext : ProgramContext
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
        string vertexSource, 
        string fragmentSource,
        bool verbose = false
    )
    {
        int tabIndex = 0;
        var vertexShader = CreateVertexShader(vertexSource, verbose, ref tabIndex);
        var fragmentShader = CreateFragmentShader(fragmentSource, verbose, ref tabIndex);
        int program = CreateProgram(vertexShader, fragmentShader, verbose, ref tabIndex);
        return program;
    }

    public override void Clear(Vec4 color)
    {
        GL.ClearColor(
            color.X,
            color.Y,
            color.Z,
            color.W
        );
    }

    public override void UseProgram(int program)
        => GL.UseProgram(program);

    /// <summary>
    /// Compile a Vertex Shader and get his id.
    /// </summary>
    private static int CreateVertexShader(
        string source,
        bool verbose,
        ref int tabIndex)
    {
        Start("Vertex Shader Creation", verbose, ref tabIndex);
        var shader = CreateShader(
            OpenTKShaderType.VertexShader,
            source, verbose, ref tabIndex
        );
        Success("Shader Created!", verbose, ref tabIndex);
        return shader;
    }
    
    /// <summary>
    /// Compile a Fragment Shader and get his id.
    /// </summary>
    private static int CreateFragmentShader(
        string source,
        bool verbose,
        ref int tabIndex)
    {
        Start("Creating Fragment Shader...", verbose, ref tabIndex);
        var shader = CreateShader(
            OpenTKShaderType.FragmentShader,
            source, verbose, ref tabIndex
        );
        Success("Shader Created!", verbose, ref tabIndex);
        return shader;
    }

    private static int CreateShader(
        OpenTKShaderType type,
        string source,
        bool verbose,
        ref int tabIndex)
    {
        Information("Getting Shader...", verbose, ref tabIndex);
        Code(source, verbose, ref tabIndex);

        var hash = source.GetHashCode();
        Information($"Hash: {hash}", verbose, ref tabIndex);

        if (shaderMap.TryGetValue(hash, out int value))
        {
            Information("Reusing other shader!", verbose, ref tabIndex);
            return value;
        }
        Information("Cache miss. Create new shader!", verbose, ref tabIndex);

        var shader = GL.CreateShader(type);
        Information($"Code: {shader}", verbose, ref tabIndex);
        Information($"Compiling Shader...", verbose, ref tabIndex);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);

        shaderMap.Add(hash, shader);

        GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
        if (code != (int)All.True)
        {
            var infoLog = GL.GetShaderInfoLog(shader);
            Error($"Error occurred in Shader({shader}) compilation: {infoLog}", verbose, ref tabIndex);
            return -1;
        }

        return shader;
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
        Start("Creating Program...", verbose, ref tabIndex);
        var programKey = (vertexShader, fragmentShader);
        if (programMap.TryGetValue(programKey, out int reusingProgram))
        {
            Information($"Reusing Program {reusingProgram}.", verbose, ref tabIndex);
            return reusingProgram;
        }

        var program = GL.CreateProgram();
        
        Information("Attaching Shaders...", verbose, ref tabIndex);
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        
        Information("Link Program...", verbose, ref tabIndex);
        GL.LinkProgram(program);

        Information("Dettaching Program...", verbose, ref tabIndex);
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