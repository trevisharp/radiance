/* Author:  Leonardo Trevisan Silio
 * Date:    10/08/2023
 */
namespace Radiance;

/// <summary>
/// A Builder to configurate the Graphics object to drawing in main screen with OpenGL.
/// </summary>
public class GraphicsBuilder
{
    internal Graphics Product { get; set; }

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
    /// Build the Graphics object.
    /// </summary>
    public Graphics Build()
    {
        Product = new Graphics(
            width,
            height
        );

        return Product;
    }

    public static implicit operator Graphics(GraphicsBuilder builder)
        => builder.Build();
}