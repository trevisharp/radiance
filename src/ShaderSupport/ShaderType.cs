/* Author:  Leonardo Trevisan Silio
 * Date:    06/08/2023
 */
namespace DuckGL.ShaderSupport;

/// <summary>
/// Represents a valid type in shader implementation.
/// </summary>
public enum ShaderType : byte
{
    None = 0,
    Float = 1,
    Vec2 = 2,
    Vec3 = 3,
    Vec4 = 4,

    Bool = 255,

    Vec = Float | Vec2 | Vec3 | Vec4
}