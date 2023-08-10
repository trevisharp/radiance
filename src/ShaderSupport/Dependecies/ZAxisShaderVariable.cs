/* Author:  Leonardo Trevisan Silio
 * Date:    10/08/2023
 */
namespace Radiance.ShaderSupport.Dependencies;

/// <summary>
/// Represent the z axis variable in a shader implementation.
/// </summary>
public class ZAxisShaderVariable : ShaderInput
{
    public override object Value => "(0, 0, 1.0)";

    public ZAxisShaderVariable()
    {
        this.DependenceType = ShaderDependenceType.Variable;
        this.Type = ShaderType.Vec3;
        this.Name = "z";
    }
}