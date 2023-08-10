/* Author:  Leonardo Trevisan Silio
 * Date:    10/08/2023
 */
namespace Radiance.ShaderSupport.Dependencies;

/// <summary>
/// Represent the x axis variable in a shader implementation.
/// </summary>
public class XAxisShaderVariable : ShaderInput
{
    public override object Value => "(1.0, 0, 0)";

    public XAxisShaderVariable()
    {
        this.DependenceType = ShaderDependenceType.Variable;
        this.Type = ShaderType.Vec3;
        this.Name = "i";
    }
}