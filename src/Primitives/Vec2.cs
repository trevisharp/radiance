/* Author:  Leonardo Trevisan Silio
 * Date:    29/08/2024
 */
using System.Globalization;

namespace Radiance.Primitives;

using System.Buffers;
using Buffers;
using Shaders;
using Shaders.Objects;

/// <summary>
/// Represents a two dimension float vector.
/// </summary>
public record Vec2(float X, float Y) : IBufferizable
{
    public void Deconstruct(out float x, out float y)
        => (x, y) = (X, Y);

    public static Vec2 operator +(Vec2 u, Vec2 v)
        => new(u.X + v.X, u.Y + v.Y);
    
    public static Vec2 operator -(Vec2 u, Vec2 v)
        => new(u.X - v.X, u.Y - v.Y);
    
    public static float operator *(Vec2 u, Vec2 v)
        => u.X * v.X + u.Y * v.Y;
    
    public static Vec2 operator *(float a, Vec2 v)
        => new(a * v.X, a * v.Y);
    
    public static Vec2 operator *(Vec2 v, float a)
        => new(a * v.X, a * v.Y);

    public static implicit operator Vec2((float x, float y) tuple)
        => new(tuple.x, tuple.y);
    
    public static implicit operator Vec2ShaderObject(Vec2 vec)
        => new($"vec2({ToTxt(vec.X)}, {ToTxt(vec.Y)})", ShaderOrigin.Global, []);

    static string ToTxt(float value)
        => value.ToString(CultureInfo.InvariantCulture);

    public int ComputeSize()
        => 2;

    public void Bufferize(float[] buffer, int index)
    {
        buffer[index] = X;
        buffer[index + 1] = Y;
    }
}