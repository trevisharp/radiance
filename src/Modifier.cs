/* Author:  Leonardo Trevisan Silio
 * Date:    23/08/2023
 */
namespace Radiance;

/// <summary>
/// Represents a keyboard modifier.
/// Equals to OpenTK.Windowing.GraphicsLibraryFramework.Modifier
/// </summary>
public enum Modifier
{
    Shift = 1,
    Control = 2,
    Alt = 4,
    Super = 8,
    CapsLock = 16,
    NumLock = 32,

    PassiveModifier = CapsLock | NumLock,
    ActiveModifier = Shift | Control | Alt | Super
}