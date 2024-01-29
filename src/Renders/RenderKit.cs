/* Author:  Leonardo Trevisan Silio
 * Date:    29/01/2024
 */
namespace Radiance.Renders;

using static Radiance.Utils;

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
}