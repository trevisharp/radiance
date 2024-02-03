/* Author:  Leonardo Trevisan Silio
 * Date:    03/02/2024
 */
namespace Radiance.Renders;

using static Radiance.Utils;

/// <summary>
/// A Kit whit some Render default implementations.
/// </summary>
public class RenderKit
{
    public readonly static RenderKit Shared = new();

    private Render fill = null;
    public dynamic Fill
    {
        get
        {
            if (fill is not null)
                return fill;
            
            fill = render((r, g, b, a) =>
            {
                color = (r, g, b, a);
                fill();
            });

            return fill;
        }
    }

    private Render draw = null;
    public dynamic Draw
    {
        get
        {
            if (draw is not null)
                return draw;
            
            draw = render((r, g, b, a) =>
            {
                color = (r, g, b, a);
                draw();
            });

            return draw;
        }
    }
}