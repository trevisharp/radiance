/* Author:  Leonardo Trevisan Silio
 * Date:    12/09/2024
 */
using System;
using System.Linq;
using System.Collections.Generic;

using StbImageSharp;
using OpenTK.Graphics.OpenGL4;

namespace Radiance.OpenGL4;

using Managers;
using Primitives;

/// <summary>
/// Represents the data and state of a shader program.
/// </summary>
public class OpenGL4ManagerContext : ShaderManager
{
    // Global OpenGL resources indexes map
    static readonly Dictionary<ImageResult, int> textureMap = [];
    static readonly List<int> bufferList = [];
    static readonly List<int> objectList = [];
    static readonly List<int> textureUnits = [];

    /// <summary>
    /// Unload all OpenGL Resources.
    /// </summary>
    public static void FreeAllResources()
    {
        foreach (var buffer in bufferList)
            GL.DeleteBuffer(buffer);
        bufferList.Clear();

        foreach (var texture in textureMap)
            GL.DeleteTexture(texture.Value);
        textureMap.Clear();

        foreach (var vertexArray in objectList)
            GL.DeleteVertexArray(vertexArray);
        objectList.Clear();
    }
    
    /// <summary>
    /// Get or set the OpenGL Program Id associated to this context.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Get the count of textures loaded on this context.
    /// </summary>
    public int TextureCount { get; private set; }

    /// <summary>
    /// Get the Vertex Array Object Id associated with this Shader.
    /// </summary>
    public int ObjectId { get; private set; } = -1;

    /// <summary>
    /// Get the total count of layout defineds on Vertex Array Object. 
    /// </summary>
    public int LayoutCount { get; private set; } = 0;

    /// <summary>
    /// Get the total offset of layouts.
    /// </summary>
    public int Offset { get; private set; } = 0;

    public override void SetProgram(int program)
        => Id = program;

    public override void SetFloat(string name, float value)
    {
        var code = GL.GetUniformLocation(Id, name);
        GL.Uniform1(code, value);
    }

    public override void SetTextureData(string name, Texture texture)
    {
        var id = ActivateImage(texture.ImageData);
        var code = GL.GetUniformLocation(Id, name);
        GL.Uniform1(code, id);
    }

    public override void Use(Polygon poly)
    {
        BindVerteArrayObject();

        poly.BufferId ??= CreateBuffer();
        BindBuffer(poly);
        SetBufferData(poly);
    }

    public override void Draw(PrimitiveType primitiveType, Polygon poly)
    {
        var openTKType = (OpenTK.Graphics.OpenGL4.PrimitiveType)primitiveType;
        GL.DrawArrays(openTKType, 0, poly.Data.Count() / 3);
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

    public override void Dispose()
    {
        // TODO: Release more data.
        DeleteVerteArrayObject(ObjectId);
        GC.SuppressFinalize(this);
    }

    private void BindVerteArrayObject()
    {
        if (ObjectId == -1)
            GenerateVertexArrayObject();
        
        GL.BindVertexArray(ObjectId);
    }

    private void GenerateVertexArrayObject()
    {
        ObjectId = GL.GenVertexArray();
        objectList.Add(ObjectId);
    }

    private static void DeleteVerteArrayObject(int objectId)
    {
        GL.DeleteVertexArray(objectId);
        objectList.Remove(objectId);
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
    
    private static int CreateBuffer()
    {
        var bufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, bufferObject);
        bufferList.Add(bufferObject);
        return bufferObject;
    }

    private static void DeleteBuffer(int bufferId)
    {
        GL.DeleteBuffer(bufferId);
        bufferList.Remove(bufferId);
    }

    private static void BindBuffer(Polygon poly)
    {
        int bufferId = poly.BufferId ?? 
            throw new Exception("A unexpected behaviour ocurred on buffer creation/binding.");
        GL.BindBuffer(
            BufferTarget.ArrayBuffer, 
            bufferId
        );
    }

    private static void SetBufferData(Polygon poly)
    {
        var data = poly.Data.ToArray();
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            data.Length * sizeof(float), data,
            BufferUsageHint.DynamicDraw
        );
    }
}