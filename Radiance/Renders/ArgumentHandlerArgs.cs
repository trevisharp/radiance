/* Author:  Leonardo Trevisan Silio
 * Date:    16/12/2024
 */
namespace Radiance.Renders;

/// <summary>
/// Arguments used on a argument handler chain.
/// </summary>
public class ArgumentHandlerArgs
{
    public required BaseRender Render { get; set; }
    public required object[] Args { get; set; }
    public required object[] NewArgs { get; set; }
}