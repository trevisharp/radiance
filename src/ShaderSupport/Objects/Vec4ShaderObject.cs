/* Author:  Leonardo Trevisan Silio
 * Date:    04/08/2023
 */
namespace DuckGL.ShaderSupport.Objects;

/// <summary>
/// Represent a Vec4 data in shader implementation.
/// </summary>
public class Vec4ShaderObject : ShaderObject
{
    public Vec4ShaderObject(
        string name = null,
        string exp = null
    ) : base(ShaderType.Vec4, name, exp)
    {

    }
}