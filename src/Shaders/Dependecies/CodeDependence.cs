/* Author:  Leonardo Trevisan Silio
 * Date:    22/01/2024
 */
using System.Collections.Generic;
using System.Linq;

namespace Radiance.Shaders.Dependencies;

/// <summary>
/// Represents a Dependence of a code definition.
/// </summary>
/// 
public class CodeDependence : OldShaderDependence
{
    private readonly string expression;
    private readonly string type;
    private readonly ShaderObject obj;
    public CodeDependence(ShaderObject obj, string name)
    {
        this.obj = obj;
        this.Name = name;
        this.type = obj.Type.ToString().ToLower();
        this.DependenceType = ShaderDependenceType.Expression;
        this.expression = obj.Expression;
    }

    public override object Value => obj;
    public override string GetHeader() => "";

    public override string GetCode()
        => $"{type} {Name} = {expression}";
}