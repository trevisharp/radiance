/* Author:  Leonardo Trevisan Silio
 * Date:    10/08/2023
 */
namespace Radiance.ShaderSupport.Dependencies;

/// <summary>
/// Represent the y axis variable in a shader implementation.
/// </summary>
public class YAxisShaderVariable : ShaderInput
{
    public override object Value => "(0, 1.0, 0)";

    public YAxisShaderVariable()
    {
        this.DependenceType = ShaderDependenceType.Variable;
        this.Type = ShaderType.Vec3;
        this.Name = "j";
    }
}