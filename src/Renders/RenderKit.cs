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
            fill ??= render((r, g, b, a) =>
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
            draw ??= render((r, g, b, a) =>
            {
                color = (r, g, b, a);
                draw();
            });

            return draw;
        }
    }
}