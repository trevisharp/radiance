/* Author:  Leonardo Trevisan Silio
 * Date:    29/01/2024
 */
namespace Radiance.Renders;

using static Radiance.Utils;

public class RenderKit
{
    public readonly static RenderKit Shared = new();

    private Render simpleFill = null;
    public dynamic SimpleFill
    {
        get
        {
            if (simpleFill is not null)
                return simpleFill;
            
            simpleFill = render((r, g, b, a) =>
            {
                verbose = true;
                color = (r, g, b, a);
                fill();
            });

            return simpleFill;
        }
    }
}