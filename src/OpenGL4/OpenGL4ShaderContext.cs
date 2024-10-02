/* Author:  Leonardo Trevisan Silio
 * Date:    25/09/2024
 */
using System;
using System.Linq;
using System.Collections.Generic;

using StbImageSharp;
using OpenTK.Graphics.OpenGL4;
using OpenTKShaderType = OpenTK.Graphics.OpenGL4.ShaderType;

namespace Radiance.OpenGL4;

using Shaders;
using Buffers;
using Contexts;
using Primitives;
using Exceptions;

/// <summary>
/// Represents the data and state of a shader program.
/// </summary>
public class OpenGL4ShaderContext : ShaderContext
{
    // Global OpenGL resources indexes map
    static readonly Dictionary<ImageResult, int> textureMap = [];
    static readonly List<int> objectList = [];
    static readonly List<int> textureUnits = [];
    static readonly Dictionary<int, int> shaderMap = [];
    static readonly Dictionary<(int, int), int> programMap = [];

    /// <summary>
    /// Unload all OpenGL Resources.
    /// </summary>
    public static void FreeAllResources()
    {
        foreach (var texture in textureMap)
            GL.DeleteTexture(texture.Value);
        textureMap.Clear();

        foreach (var vertexArray in objectList)
            GL.DeleteVertexArray(vertexArray);
        objectList.Clear();
        
        GL.UseProgram(0);
        foreach (var program in programMap)
            GL.DeleteProgram(program.Value);
        programMap.Clear();

        foreach (var shaderKey in shaderMap)
            GL.DeleteShader(shaderKey.Value);
        shaderMap.Clear();
    }
    
    /// <summary>
    /// Get or set the OpenGL Program Id associated to this context.
    /// </summary>
    public int? ProgramId { get; private set; } = null;

    /// <summary>
    /// Get the count of textures loaded on this context.
    /// </summary>
    public int TextureCount { get; private set; } = 0;

    /// <summary>
    /// Get the Vertex Array Object Id associated with this Shader.
    /// </summary>
    public int? ObjectId { get; private set; } = null;

    /// <summary>
    /// Get the total count of layout defineds on Vertex Array Object. 
    /// </summary>
    public int LayoutCount { get; private set; } = 0;

    /// <summary>
    /// Get the total offset of layouts.
    /// </summary>
    public int Offset { get; private set; } = 0;

    public override void SetFloat(string name, float value)
    {
        var program = ProgramId ?? throw new UncreatedProgramException();
        var code = GL.GetUniformLocation(program, name);
        GL.Uniform1(code, value);
    }

    public override void SetTextureData(string name, Texture texture)
    {
        var program = ProgramId ?? throw new UncreatedProgramException();
        var id = ActivateImage(texture.ImageData);
        var code = GL.GetUniformLocation(program, name);
        GL.Uniform1(code, id);
    }

    public override void SetVec(string name, float x, float y)
    {
        var program = ProgramId ?? throw new UncreatedProgramException();
        var code = GL.GetUniformLocation(program, name);
        GL.Uniform2(code, x, y);
    }

    public override void SetVec(string name, float x, float y, float z)
    {
        var program = ProgramId ?? throw new UncreatedProgramException();
        var code = GL.GetUniformLocation(program, name);
        GL.Uniform3(code, x, y, z);
    }

    public override void SetVec(string name, float x, float y, float z, float w)
    {
        var program = ProgramId ?? throw new UncreatedProgramException();
        var code = GL.GetUniformLocation(program, name);
        GL.Uniform4(code, x, y, z, w);
    }

    public override void Use(IBufferedData data)
    {
        BindVerteArrayObject();
        BufferManager.Use(data);
    }

    public override void Draw(PrimitiveType primitiveType, IBufferedData data)
    {
        var openTKType = (OpenTK.Graphics.OpenGL4.PrimitiveType)primitiveType;
        GL.DrawArrays(openTKType, 0, data.Vertices);
    }

    public override void AddLayout(int size)
    {
        BindVerteArrayObject();

        var stride = size * sizeof(float);
        var type = VertexAttribPointerType.Float;

        GL.VertexAttribPointer(LayoutCount, 3, type, false, stride, Offset);
        GL.EnableVertexAttribArray(LayoutCount);
        Offset += 3 * sizeof(float);
        LayoutCount++;
    }

    public override void CreateProgram(ShaderPair pair, bool verbose = false)
    {
        int tabIndex = 0;
        var vertexShader = CreateVertexShader(pair.VertexShader, verbose, ref tabIndex);
        var fragmentShader = CreateFragmentShader(pair.FragmentShader, verbose, ref tabIndex);
        ProgramId = CreateProgram(vertexShader, fragmentShader, verbose, ref tabIndex);
    }

    public override void UseProgram()
    {
        var program = ProgramId ?? throw new UncreatedProgramException();
        GL.UseProgram(program);
    }

    public override void Dispose()
    {
        DeleteVerteArrayObject(ObjectId);
        DeleteProgram(ProgramId);
        GC.SuppressFinalize(this);
    }

    private static void DeleteProgram(int? programId)
    {
        if (programId is null)
            return;
        int program = programId.Value;

        GL.UseProgram(0);
        GL.DeleteProgram(program);
        
        if (!programMap.ContainsValue(program))
            return;
        
        var keyPair = programMap
            .FirstOrDefault(p => p.Value == program);
        programMap.Remove(keyPair.Key);
    }

    private void BindVerteArrayObject()
    {
        if (!ObjectId.HasValue)
            GenerateVertexArrayObject();
        
        GL.BindVertexArray(ObjectId ?? -1);
    }

    private void GenerateVertexArrayObject()
    {
        ObjectId = GL.GenVertexArray();
        objectList.Add(ObjectId ?? -1);
    }

    private static void DeleteVerteArrayObject(int? objectId)
    {
        if (!objectId.HasValue)
            return;
        
        GL.DeleteVertexArray(objectId.Value);
        objectList.Remove(objectId.Value);
    }

    private int ActivateImage(ImageResult image)
    {
        int handle = GetTextureHandle(image);
        var index = textureUnits.IndexOf(handle);
        int id = index > -1 ? index : TextureCount++;
        
        if (textureUnits.Count < TextureCount)
            textureUnits.Add(handle);

        GL.ActiveTexture(TextureUnit.Texture0 + id);
        GL.BindTexture(TextureTarget.Texture2D, handle);
        
        textureUnits[id] = handle;
        return id;
    }

    private static int GetTextureHandle(ImageResult image)
    {
        if (textureMap.TryGetValue(image, out int value))
            return value;
        
        int handle = InitImageData(image);
        textureMap.Add(image, handle);
        return handle;
    }

    private static int InitImageData(ImageResult image)
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