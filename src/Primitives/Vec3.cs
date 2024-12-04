/* Author:  Leonardo Trevisan Silio
 * Date:    29/08/2024
 */
using System.Globalization;

namespace Radiance.Primitives;

using Buffers;
using Shaders;
using Shaders.Objects;

/// <summary>
/// Represents a three dimension float vector.
/// </summary>
public record Vec3(float X, float Y, float Z) : IBufferizable
{
    public void Deconstruct(out float x, out float y, out float z)
        => (x, y, z) = (X, Y, Z);
    
    public static Vec3 operator +(Vec3 u, Vec3 v)
        => new(u.X + v.X, u.Y + v.Y, u.Z + v.Z);
    
    public static Vec3 operator -(Vec3 u, Vec3 v)
        => new(u.X - v.X, u.Y - v.Y, u.Z - v.Z);
    
    public static float operator *(Vec3 u, Vec3 v)
        => u.X * v.X + u.Y * v.Y + u.Z * v.Z;
    
    public static Vec3 operator *(float a, Vec3 v)
        => new(a * v.X, a * v.Y, a * v.Z);
    
    public static Vec3 operator *(Vec3 v, float a)
        => new(a * v.X, a * v.Y, a * v.Z);
    
    public static implicit operator Vec3((float x, float y, float z) tuple)
        => new(tuple.x, tuple.y, tuple.z);
    
    public static implicit operator Vec3ShaderObject(Vec3 vec)
        => new($"vec3({ToTxt(vec.X)}, {ToTxt(vec.Y)}, {ToTxt(vec.Z)})", ShaderOrigin.Global, []);

    static string ToTxt(float value)
        => value.ToString(CultureInfo.InvariantCulture);
        
    public int ComputeSize()
        => 3;

    public void Bufferize(float[] buffer, int index)
    {
        buffer[index] = X;
        buffer[index + 1] = Y;
        buffer[index + 2] = Z;
    }
}