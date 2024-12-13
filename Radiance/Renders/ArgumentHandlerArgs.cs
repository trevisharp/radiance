/* Author:  Leonardo Trevisan Silio
 * Date:    13/12/2024
 */
namespace Radiance.Renders;

/// <summary>
/// Arguments used on a argument handler chain.
/// </summary>
public record ArgumentHandlerArgs(
    object[] Args,
    object[] NewArgs
);