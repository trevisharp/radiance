/* Author:  Leonardo Trevisan Silio
 * Date:    29/08/2024
 */
using System.Globalization;

namespace Radiance.Primitives;

using Buffers;
using Shaders;
using Shaders.Objects;

/// <summary>
/// Represents a four dimension float vector.
/// </summary>
public record Vec4(float X, float Y, float Z, float W) : IBufferizable
{
    public void Deconstruct(out float x, out float y, out float z, out float w)
        => (x, y, z, w) = (X, Y, Z, W);

    public static implicit operator Vec4((float x, float y, float z, float w) tuple)
        => new(tuple.x, tuple.y, tuple.z, tuple.w);
    
    public static Vec4 operator +(Vec4 u, Vec4 v)
        => new(u.X + v.X, u.Y + v.Y, u.Z + v.Z, u.W + v.W);
    
    public static Vec4 operator -(Vec4 u, Vec4 v)
        => new(u.X - v.X, u.Y - v.Y, u.Z - v.Z, u.W - v.W);
    
    public static float operator *(Vec4 u, Vec4 v)
        => u.X * v.X + u.Y * v.Y + u.Z * v.Z + u.W * v.W;
    
    public static Vec4 operator *(float a, Vec4 v)
        => new(a * v.X, a * v.Y, a * v.Z, a * v.W);
    
    public static Vec4 operator *(Vec4 v, float a)
        => new(a * v.X, a * v.Y, a * v.Z, a * v.W);
    
    public static implicit operator Vec4ShaderObject(Vec4 vec)
        => new($"vec4({ToTxt(vec.X)}, {ToTxt(vec.Y)}, {ToTxt(vec.Z)}, {ToTxt(vec.W)})", ShaderOrigin.Global, []);
    
    static string ToTxt(float value)
        => value.ToString(CultureInfo.InvariantCulture);
    
    public int ComputeSize()
        => 4;

    public void Bufferize(float[] buffer, int index)
    {
        buffer[index] = X;
        buffer[index + 1] = Y;
        buffer[index + 2] = Z;
        buffer[index + 3] = W;
    }
}