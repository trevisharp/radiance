/* Author:  Leonardo Trevisan Silio
 * Date:    19/01/2024
 */
namespace Radiance.Data;

/// <summary>
/// Represents a four dimension float vector.
/// </summary>
public record Vec4(float X, float Y, float Z, float W)
{
    public void Deconstruct(out float x, out float y, out float z, out float w)
    {
        x = this.X;
        y = this.Y;
        z = this.Z;
        w = this.W;
    }
    public static implicit operator Vec4((float x, float y, float z, float w) tuple)
        => new Vec4(tuple.x, tuple.y, tuple.z, tuple.w);
}