/* Author:  Leonardo Trevisan Silio
 * Date:    24/01/2024
 */
namespace Radiance.Shaders;

/// <summary>
/// Represents a valid type in a shader.
/// </summary>
public class ShaderType(string typeName)
{
    public readonly string TypeName = typeName;

    public static readonly ShaderType Float = new("float");
    public static readonly ShaderType Vec2 = new("vec2");
    public static readonly ShaderType Vec3 = new("vec3");
    public static readonly ShaderType Vec4 = new("vec4");
    public static readonly ShaderType Bool = new("bool");
    public static readonly ShaderType Sampler = new("sampler2D");
}