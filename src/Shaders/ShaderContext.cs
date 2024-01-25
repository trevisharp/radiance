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
    static Dictionary<int, int> shaderMap = new();
    static Dictionary<(int, int), int> programMap = new();
    static Dictionary<ImageResult, int> textureMap = new();
    static List<int> bufferList = new();
    static List<int> vertexArrayList = new();
    static List<int> textureUnits = new();

    /// <summary>
    /// Unload all OpenGL Resources.
    /// </summary>
    public static void FreeAllResources()
    {
        GL.UseProgram(0);
        foreach (var program in programMap)
            GL.DeleteProgram(program.Value);
        programMap.Clear();

        foreach (var buffer in bufferList)
            GL.DeleteBuffer(buffer);
        bufferList.Clear();

        foreach (var shaderKey in shaderMap)
            GL.DeleteShader(shaderKey.Value);
        shaderMap.Clear();

        foreach (var texture in textureMap)
            GL.DeleteTexture(texture.Value);
        textureMap.Clear();

        foreach (var vertexArray in vertexArrayList)
            GL.DeleteVertexArray(vertexArray);
        vertexArrayList.Clear();
    }
    
    public int Program { get; set; }
    public int TextureCount { get; set; }
    
    public void SetUniformFloat(string name, float value)
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
}