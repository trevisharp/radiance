/* Author:  Leonardo Trevisan Silio
 * Date:    25/01/2024
 */
using System.Collections.Generic;

using StbImageSharp;
using OpenTK.Graphics.OpenGL4;

namespace Radiance.Shaders;

using System.Linq;
using System.Net.Sockets;
using Data;
using Dependencies;

/// <summary>
/// Represents the state of shader generation.
/// </summary>
public class ShaderContext
{
    // Global OpenGL resources indexes map
    static Dictionary<ImageResult, int> textureMap = new();
    static List<int> bufferList = new();
    static List<int> vertexArrayList = new();
    static List<int> textureUnits = new();

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
    
    public int Program { get; set; }
    public int TextureCount { get; set; }
    
    public void SetFloat(string name, float value)
    {
        var code = GL.GetUniformLocation(Program, name);
        GL.Uniform1(code, value);
    }

    public void SetTextureData(Texture texture, string name)
    {
        var id = activateImage(texture.ImageData);
        var code = GL.GetUniformLocation(Program, name);
        GL.Uniform1(code, id);
    }

    public void CreateResources(Polygon poly)
    {
        if (poly.VertexObjectArray > -1 && poly.Buffer > -1)
            return;

        updateResources(poly, true, true);
        poly.OnChange += (bufferBreak, layoutBreak) =>
            updateResources(poly, bufferBreak, layoutBreak);
    }

    public void Use(Polygon poly)
    {            
        bindVertexArray(poly);
        bindBuffer(poly);
    }

    private int activateImage(ImageResult image)
    {
        int id = -1;
        int handle = getTextureHandle(image);
        var index = textureUnits.IndexOf(handle);

        id = index > -1 ? index : TextureCount++;
        
        if (textureUnits.Count < TextureCount)
        {
            textureUnits.Add(handle);
            GL.ActiveTexture(TextureUnit.Texture0 + id);
        }

        textureUnits[id] = handle;
        GL.BindTexture(TextureTarget.Texture2D, handle);
        return id;
    }

    private int getTextureHandle(ImageResult image)
    {
        if (textureMap.ContainsKey(image))
            return textureMap[image];
        
        int handle = initImageData(image);
        textureMap.Add(image, handle);
        return handle;
    }

    private int initImageData(ImageResult image)
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

    private int createVertexArray(Polygon data)
    {
        int vertexObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexObject);

        int total = data.Layouts.Sum(layout => layout.size);
        var stride = total * sizeof(float);
        var type = VertexAttribPointerType.Float;

        int i = 0;
        int offset = 0;
        foreach (var layout in data.Layouts)
        {
            GL.VertexAttribPointer(i, layout.size, type, false, stride, offset);
            GL.EnableVertexAttribArray(i);
            offset += layout.size * sizeof(float);
            i++;
        }

        vertexArrayList.Add(vertexObject);
        return vertexObject;
    }
    
    private int createBuffer()
    {
        var bufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, bufferObject);
        bufferList.Add(bufferObject);
        return bufferObject;
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
}