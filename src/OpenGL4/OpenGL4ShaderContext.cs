/* Author:  Leonardo Trevisan Silio
 * Date:    29/10/2024
 */
#define DEBUGOPENGL4
#undef DEBUGOPENGL4 //remove to DEBUG mode

using System;
using System.Linq;
using System.Collections.Generic;

using StbImageSharp;
using OpenTK.Graphics.OpenGL4;
using OpenTKShaderType = OpenTK.Graphics.OpenGL4.ShaderType;

namespace Radiance.OpenGL4;

using Shaders;
using Buffers;
using Internal;
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
    /// Get the total count of layout defineds on Vertex Array Object. 
    /// </summary>
    public int LayoutCount { get; private set; } = 0;

    /// <summary>
    /// Get the total offset of layouts.
    /// </summary>
    public int TotalOffset { get; private set; } = 0;

    readonly FeatureMap<VertexArrayObject> vertexArrayMap = new();
    record VertexArrayObject(int[] Buffers, int Id);

    readonly Queue<LayoutInfo> layoutConfigs = [];
    record LayoutInfo(
        int Index, int Size, int Offset, 
        VertexAttribPointerType Type
    );

    public override void SetFloat(string name, float value)
    {
        var program = ProgramId ?? throw new UncreatedProgramException();
        var code = GL.GetUniformLocation(program, name);
        GL.Uniform1(code, value);

        #if DEBUGOPENGL4
        Console.WriteLine($"GL.GetUniformLocation({program}, {name}) = {code}");
        Console.WriteLine($"GL.Uniform4({code}, ...)");
        #endif
    }

    public override void SetTextureData(string name, Texture texture)
    {
        var program = ProgramId ?? throw new UncreatedProgramException();
        var id = ActivateImage(texture.ImageData);
        var code = GL.GetUniformLocation(program, name);
        GL.Uniform1(code, id);

        #if DEBUGOPENGL4
        Console.WriteLine($"GL.GetUniformLocation({program}, {name}) = {code}");
        Console.WriteLine($"GL.Uniform1({code}, ...)");
        #endif
    }

    public override void SetVec(string name, float x, float y)
    {
        var program = ProgramId ?? throw new UncreatedProgramException();
        var code = GL.GetUniformLocation(program, name);
        GL.Uniform2(code, x, y);

        #if DEBUGOPENGL4
        Console.WriteLine($"GL.GetUniformLocation({program}, {name}) = {code}");
        Console.WriteLine($"GL.Uniform2({code}, ...)");
        #endif
    }

    public override void SetVec(string name, float x, float y, float z)
    {
        var program = ProgramId ?? throw new UncreatedProgramException();
        var code = GL.GetUniformLocation(program, name);
        GL.Uniform3(code, x, y, z);

        #if DEBUGOPENGL4
        Console.WriteLine($"GL.GetUniformLocation({program}, {name}) = {code}");
        Console.WriteLine($"GL.Uniform3({code}, ...)");
        #endif
    }

    public override void SetVec(string name, float x, float y, float z, float w)
    {
        var program = ProgramId ?? throw new UncreatedProgramException();
        var code = GL.GetUniformLocation(program, name);
        GL.Uniform4(code, x, y, z, w);

        #if DEBUGOPENGL4
        Console.WriteLine($"GL.GetUniformLocation({program}, {name}) = {code}");
        Console.WriteLine($"GL.Uniform4({code}, ...)");
        #endif
    }

    public override void AddLayout(int size, DataType dataType)
    {
        var stride = size * sizeof(float);

        var openGLType = dataType switch
        {
            DataType.Float => VertexAttribPointerType.Float,
            _ => throw new Exception("Invalid layout type")
        };

        layoutConfigs.Enqueue(new(LayoutCount, size, TotalOffset, openGLType));

        TotalOffset += stride;
        LayoutCount++;
    }

    public override void CreateProgram(ShaderPair pair, bool verbose = false)
    {
        int tabIndex = 0;
        var vertexShader = CreateVertexShader(pair.VertexShader, verbose, ref tabIndex);
        var fragmentShader = CreateFragmentShader(pair.FragmentShader, verbose, ref tabIndex);
        ProgramId = CreateProgram(vertexShader, fragmentShader, verbose, ref tabIndex);
    }

    public override void Draw(PrimitiveType primitiveType, IBufferedData data)
    {
        var openTKType = (OpenTK.Graphics.OpenGL4.PrimitiveType)primitiveType;
        GL.DrawArrays(openTKType, 0, data.Count);

        #if DEBUGOPENGL4
        Console.WriteLine($"GL.DrawArrays(...)");
        #endif
    }

    public override void UseProgram()
    {
        var program = ProgramId ?? throw new UncreatedProgramException();
        GL.UseProgram(program);

        #if DEBUGOPENGL4
        Console.WriteLine($"GL.UseProgram({program})");
        #endif
    }

    public override void FirstConfiguration() { }

    public override void Dispose()
    {
        foreach (var vertexArray in vertexArrayMap)
            DeleteVerteArrayObject(vertexArray.Id);
        DeleteProgram(ProgramId);
        GC.SuppressFinalize(this);
    }

    public override void InitArgs(object[] args)
    {
        var bufferedData = args
            .Where(arg => arg is IBufferedData)
            .Select(arg => (IBufferedData)arg)
            .ToArray();
        
        foreach (var data in bufferedData)
            Buffer.CreateIfNotExists(data);
        
        UseArgs(bufferedData);
    }

    public override void UseArgs(object[] args)
    {
        var bufferedData = args
            .Where(arg => arg is IBufferedData)
            .Select(arg => (IBufferedData)arg)
            .ToArray();
        
        UseArgs(bufferedData);
    }

    void UseArgs(IBufferedData[] bufferedData)
    {
        int id = GetVertexArrayObject(bufferedData);
        BindVerteArrayObject(id);
    }

    static void BindVerteArrayObject(int id)
    {
        GL.BindVertexArray(id);
        
        #if DEBUGOPENGL4
        Console.WriteLine($"GL.BindVertexArray({id})");
        #endif
    }

    int GetVertexArrayObject(IBufferedData[] bufferedData)
    {
        var buffers = 
            from data in bufferedData
            select data.Buffer?.BufferId ?? -1;
        
        if (buffers.Any(id => id == -1))
            throw new UnbufferedDataExcetion();
        int[] ids = [ ..buffers ];

        var vao = vertexArrayMap.Get(ids);
        if (vao is not null)
            return vao.Id;
        
        var id = GenerateVertexArrayObject();
        vertexArrayMap.Add(ids, new(ids, id));

        ConfigureVertexArrayObject(id, ids);

        return id;
    }

    static int GenerateVertexArrayObject()
    {
        var id = GL.GenVertexArray();

        #if DEBUGOPENGL4
        Console.WriteLine($"GL.GenVertexArray() = {id}");
        #endif

        return id;
    }

    void ConfigureVertexArrayObject(int id, int[] buffers)
    {
        BindVerteArrayObject(id);

        foreach (var (config, buffer) in layoutConfigs.Zip(buffers))
        {
            var (index, size, offset, type) = config;
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.VertexAttribPointer(index, size, type, false, TotalOffset, offset);
            GL.EnableVertexAttribArray(index);
            
            #if DEBUGOPENGL4
            Console.WriteLine($"GL.BindBuffer(..., {buffer})");
            Console.WriteLine($"GL.VertexAttribPointer({index}, {size}, ..., {TotalOffset}, {offset})");
            Console.WriteLine($"GL.EnableVertexAttribArray({index})");
            #endif
        }
    }

    static void DeleteProgram(int? programId)
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
        
        #if DEBUGOPENGL4
        Console.WriteLine($"GL.UseProgram(0)");
        Console.WriteLine($"GL.DeleteProgram({program})");
        #endif
    }

    static void DeleteVerteArrayObject(int? objectId)
    {
        if (!objectId.HasValue)
            return;
        
        GL.DeleteVertexArray(objectId.Value);

        #if DEBUGOPENGL4
        Console.WriteLine($"GL.DeleteVertexArray({objectId.Value})");
        #endif
    }

    int ActivateImage(ImageResult image)
    {
        int handle = GetTextureHandle(image);
        var index = textureUnits.IndexOf(handle);
        int id = index > -1 ? index : TextureCount++;
        
        if (textureUnits.Count < TextureCount)
            textureUnits.Add(handle);

        GL.ActiveTexture(TextureUnit.Texture0 + id);
        GL.BindTexture(TextureTarget.Texture2D, handle);
        textureUnits[id] = handle;
        
        #if DEBUGOPENGL4
        Console.WriteLine($"GL.ActiveTexture(...)");
        Console.WriteLine($"GL.BindTexture(..., {handle})");
        #endif
        
        return id;
    }

    static int GetTextureHandle(ImageResult image)
    {
        if (textureMap.TryGetValue(image, out int value))
            return value;
        
        int handle = InitImageData(image);
        textureMap.Add(image, handle);
        return handle;
    }

    static int InitImageData(ImageResult image)
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

        #if DEBUGOPENGL4
        Console.WriteLine($"GL.BindTexture(TextureTarget.Texture2D, {handle})");
        Console.WriteLine($"GL.TexImage2D(...)");
        Console.WriteLine($"GL.TexParameter(...)");
        Console.WriteLine($"GL.TexParameter(...)");
        Console.WriteLine($"GL.TexParameter(...)");
        Console.WriteLine($"GL.TexParameter(...)");
        Console.WriteLine($"GL.GenerateMipmap(...)");
        #endif

        return handle;
    }
    
    static int CreateVertexShader(
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
    
    static int CreateFragmentShader(
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

    static int CreateShader(
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

        #if DEBUGOPENGL4
        Console.WriteLine($"GL.CreateShader(...) = {shaderId}");
        Console.WriteLine($"GL.ShaderSource({shaderId}, ...)");
        Console.WriteLine($"GL.CompileShader({shaderId})");
        #endif

        return shaderId;
    }
    
    static int CreateProgram(
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

        #if DEBUGOPENGL4
        Console.WriteLine($"GL.CreateProgram() = {program}");
        Console.WriteLine($"GL.AttachShader({program}, {vertexShader})");
        Console.WriteLine($"GL.AttachShader({program}, {fragmentShader})");
        Console.WriteLine($"GL.LinkProgram({program})");
        Console.WriteLine($"GL.DetachShader({program}, {vertexShader})");
        Console.WriteLine($"GL.DetachShader({program}, {fragmentShader})");
        #endif
        return program;
    }

    static void BindBuffer(int id)
    {
        #if DEBUGOPENGL4
        Console.WriteLine($"GL.BindBuffer({id})");
        #endif

        GL.BindBuffer(BufferTarget.ArrayBuffer, id);
    }

    static int CreateBuffer()
    {
        int id = GL.GenBuffer();

        #if DEBUGOPENGL4
        Console.WriteLine($"GL.GenBuffer() = {id}");
        #endif

        return id;
    }

    static void Delete(int id)
    {
        #if DEBUGOPENGL4
        Console.WriteLine($"GL.DeleteBuffer({id})");
        #endif

        GL.DeleteBuffer(id);
    }

    static void Store(float[] data, bool dynamicData)
    {
        #if DEBUGOPENGL4
        Console.WriteLine("GL.BufferData(...)");
        #endif

        GL.BufferData(
            BufferTarget.ArrayBuffer, data.Length * sizeof(float), data,
            dynamicData ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw
        );
    }
}