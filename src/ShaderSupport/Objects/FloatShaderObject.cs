/* Author:  Leonardo Trevisan Silio
 * Date:    04/08/2023
 */
namespace Radiance.ShaderSupport.Objects;

/// <summary>
/// Represent a Float data in shader implementation.
/// </summary>
public class FloatShaderObject : ShaderObject
{
    public FloatShaderObject(
        string name = null,
        string exp = null
    ) : base(ShaderType.Float, name, exp)
    {
        
    }
}