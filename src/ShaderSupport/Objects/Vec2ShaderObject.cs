/* Author:  Leonardo Trevisan Silio
 * Date:    04/08/2023
 */
namespace Radiance.ShaderSupport.Objects;

/// <summary>
/// Represent a Vec2 data in shader implementation.
/// </summary>
public class Vec2ShaderObject : ShaderObject
{
    public Vec2ShaderObject(
        string name = null,
        string exp = null
    ) : base(ShaderType.Vec2, name, exp)
    {
        
    }   
}