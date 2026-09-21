/* Author:  Leonardo Trevisan Silio
 * Date:    20/09/2026
 */
namespace Radiance.Internal;

/// <summary>
/// How the closed rings of a polygon decide what is filled and what is a hole.
/// </summary>
public enum FillRule
{
    /// <summary>
    /// A ring inside an odd number of other rings is a hole, whatever
    /// direction it runs. Natural for shapes written by hand.
    /// </summary>
    Nesting,

    /// <summary>
    /// A clockwise ring is filled and a counterclockwise ring is a hole, as
    /// font files declare them. Required when rings overlap instead of
    /// nesting, where being inside cannot classify anything.
    /// </summary>
    Winding
}
