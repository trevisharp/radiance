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
    private ShaderContext vertexContext = null;
    private ShaderContext fragmentContext = null;
    private int[] layout = null;
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
        vertexContext = Shaders.GetContext();

        var shader = ShaderConverter.ToShader(vertexContext);
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
        fragmentContext = Shaders.GetContext();

        var shader = ShaderConverter.ToShader(fragmentContext);
        SetFragmentShader(shader);

        return this;
    }

    /// <summary>
    /// Set layout information.
    /// </summary>
    public GraphicsBuilder SetLayout(params int[] layout)
    {
        this.layout = layout;
        return this;
    }

    /// <summary>
    /// Build the Graphics object.
    /// </summary>
    public Graphics Build()
    {
        if (vertexContext is null && layout is null)
            layout = new int[] { 3 };
        else if (layout is null)
            layout = 
                vertexContext.Layout
                .Select(x => (int)x.Type)
                .ToArray();

        return new Graphics(
            width,
            height,
            vertexShader,
            fragmentShader,
            layout
        )
    }
}