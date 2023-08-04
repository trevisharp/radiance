/* Author:  Leonardo Trevisan Silio
 * Date:    04/08/2023
 */
using System;

namespace DuckGL;

using ShaderSupport;

/// <summary>
/// A Builder to configurate the Graphics object to drawing in main screen with OpenGL.
/// </summary>
public class GraphicsBuilder
{
    private string vertexShader = string.Empty;
    private string fragmentShader = string.Empty;
    private int width = -1;
    private int height = -1;

    /// <summary>
    /// Set Width of the Screen used by the Graphics.
    /// </summary>
    public GraphicsBuilder SetWidth(int value)
    {
        this.width = value;
        return this;
    }

    /// <summary>
    /// Set Height of the Screen used by the Graphics.
    /// </summary>
    public GraphicsBuilder SetHeight(int value)
    {
        this.height = value;
        return this;
    }

    /// <summary>
    /// Set the Vertex Shader Source.
    /// </summary>
    public GraphicsBuilder SetVertexShader(string value)
    {
        this.vertexShader = value;
        return this;
    }

    /// <summary>
    /// Set the Fragment Shader Source.
    /// </summary>
    public GraphicsBuilder SetFragmentShader(string value)
    {
        this.fragmentShader = value;
        return this;
    }

    /// <summary>
    /// Set the Vertex Shader Source.
    /// </summary>
    public GraphicsBuilder SetVertexShader(Action value)
    {
        Shaders.ResetContext();
        value();
        var ctx = Shaders.GetContext();

        var shader = ShaderConverter.ToShader(ctx);
        SetVertexShader(shader);

        return this;
    }

    /// <summary>
    /// Set the Fragment Shader Source.
    /// </summary>
    public GraphicsBuilder SetFragmentShader(Action value)
    {
        Shaders.ResetContext();
        value();
        var ctx = Shaders.GetContext();

        var shader = ShaderConverter.ToShader(ctx);
        SetFragmentShader(shader);

        return this;
    }

    /// <summary>
    /// Build the Graphics object.
    /// </summary>
    public Graphics Build()
    {
        System.Console.WriteLine(vertexShader);   
        System.Console.WriteLine();   
        System.Console.WriteLine(fragmentShader);   
        throw new NotImplementedException();
    }
}