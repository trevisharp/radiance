/* Author:  Leonardo Trevisan Silio
 * Date:    17/02/2024
 */
namespace Radiance.Data;

using Shaders;
using Shaders.Objects;

/// <summary>
/// Represents a two dimension float vector.
/// </summary>
public record Vec2(float X, float Y)
{
    public void Deconstruct(out float x, out float y)
    {
        x = this.X;
        y = this.Y;
    }

    public static Vec2 operator +(Vec2 u, Vec2 v)
        => new Vec2(u.X + v.X, u.Y + v.Y);
    public static Vec2 operator -(Vec2 u, Vec2 v)
        => new Vec2(u.X - v.X, u.Y - v.Y);
    public static float operator *(Vec2 u, Vec2 v)
        => u.X * v.X + u.Y * v.Y;
    public static Vec2 operator *(float a, Vec2 v)
        => new Vec2(a * v.X, a * v.Y);
    public static Vec2 operator *(Vec2 v, float a)
        => new Vec2(a * v.X, a * v.Y);

    public static implicit operator Vec2((float x, float y) tuple)
        => new Vec2(tuple.x, tuple.y);
    
    public static implicit operator Vec2ShaderObject(Vec2 vec)
        => new Vec2ShaderObject($"vec2({vec.X}, {vec.Y})", ShaderOrigin.Global, []);
}