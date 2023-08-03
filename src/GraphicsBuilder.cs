/* Author:  Leonardo Trevisan Silio
 * Date:    03/08/2023
 */
using System;
using System.Linq.Expressions;

namespace DuckGL;

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
        throw new NotImplementedException();
    }

    /// <summary>
    /// Set the Fragment Shader Source.
    /// </summary>
    public GraphicsBuilder SetFragmentShader(Action value)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Build the Graphics object.
    /// </summary>
    public Graphics Build()
    {
        throw new NotImplementedException();
    }
}