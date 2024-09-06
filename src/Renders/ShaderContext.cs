/* Author:  Leonardo Trevisan Silio
 * Date:    29/08/2024
 */
using System.Linq;
using System.Collections.Generic;

using StbImageSharp;
using OpenTK.Graphics.OpenGL4;

namespace Radiance.Renders;

using Primitives;

/// <summary>
/// Represents the data and state of a shader program.
/// </summary>
public class ShaderContext
{
    // Global OpenGL resources indexes map
    static readonly Dictionary<ImageResult, int> textureMap = [];
    static readonly List<int> bufferList = [];
    static readonly List<int> vertexArrayList = [];
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

        foreach (var vertexArray in vertexArrayList)
            GL.DeleteVertexArray(vertexArray);
        vertexArrayList.Clear();
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
    /// Set a uniform with a name to a specific value.
    /// </summary>
    public void SetFloat(string name, float value)
    {
        var code = GL.GetUniformLocation(Id, name);
        GL.Uniform1(code, value);
    }

    /// <summary>
    /// Set a image uniform with a name to a specific value.
    /// </summary>
    public void SetTextureData(string name, Texture texture)
    {
        var id = ActivateImage(texture.ImageData);
        var code = GL.GetUniformLocation(Id, name);
        GL.Uniform1(code, id);
    }
    
    public void CreateResources(Polygon poly)
    {
        if (poly.VertexObjectArray > -1 && poly.Buffer > -1)
            return;

        UpdateResources(poly, true, true);
        poly.OnChange += (bufferBreak, layoutBreak) =>
            UpdateResources(poly, bufferBreak, layoutBreak);
    }

    public void Use(Polygon poly)
    {
        BindVertexArray(poly);
        BindBuffer(poly);
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

    // TODO: Consider multi layout or simplify abstraction
    private static int CreateVertexArray(Polygon data)
    {
        int vertexObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexObject);

        int total = 3;
        var stride = total * sizeof(float);
        var type = VertexAttribPointerType.Float;

        int i = 0;
        int offset = 0;
        GL.VertexAttribPointer(i, 3, type, false, stride, offset);
        GL.EnableVertexAttribArray(i);
        offset += 3 * sizeof(float);
        i++;

        vertexArrayList.Add(vertexObject);
        return vertexObject;
    }
    
    private static int CreateBuffer()
    {
        var bufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, bufferObject);
        bufferList.Add(bufferObject);
        return bufferObject;
    }

    private static void BindBuffer(Polygon poly)
    {
        GL.BindBuffer(
            BufferTarget.ArrayBuffer, 
            poly.Buffer
        );
    }

    private static void BindVertexArray(Polygon poly)
    {
        GL.BindVertexArray(
            poly.VertexObjectArray
        );
    }

    private static void UpdateResources(Polygon poly, bool bufferBreak, bool layoutBreak)
    {
        if (bufferBreak)
        {
            if (poly.Buffer > -1)
                GL.DeleteBuffer(poly.Buffer);

            int buffer = CreateBuffer();
            poly.Buffer = buffer;
        }
        BindBuffer(poly);

        if (layoutBreak)
        {
            if (poly.VertexObjectArray > -1)
                GL.DeleteVertexArray(poly.VertexObjectArray);

            int vertexArray = CreateVertexArray(poly);
            poly.VertexObjectArray = vertexArray;
        }
        BindVertexArray(poly);

        var data = poly.Data.ToArray();
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            data.Length * sizeof(float), data, 
            BufferUsageHint.DynamicDraw
        );
    }
}