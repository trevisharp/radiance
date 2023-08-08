/* Author:  Leonardo Trevisan Silio
 * Date:    04/08/2023
 */
namespace Radiance.ShaderSupport.Objects;

/// <summary>
/// Represent a Vec3 data in shader implementation.
/// </summary>
public class Vec3ShaderObject : ShaderObject
{
    public Vec3ShaderObject(
        string name = null,
        string exp = null
    ) : base(ShaderType.Vec3, name, exp)
    {
        
    }   
}