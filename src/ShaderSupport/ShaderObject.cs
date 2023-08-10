/* Author:  Leonardo Trevisan Silio
 * Date:    10/08/2023
 */
using System.Collections.Generic;

namespace Radiance.ShaderSupport;

/// <summary>
/// Represents any data in a shader implementation.
/// </summary>
public class ShaderObject
{
    public virtual IEnumerable<ShaderDependence> Dependecies { get; set; }
    public virtual ShaderType Type { get; set; }
    public virtual string Expression { get; set; }

    public static implicit operator ShaderObject(float value) => value;
    public static implicit operator ShaderObject(double value) => (float)value;
    public static implicit operator ShaderObject(int value) => value;
    public static implicit operator ShaderObject(bool value) => value;
    public static implicit operator ShaderObject((float x, float y) value) => value;
    public static implicit operator ShaderObject((float x, float y, float z) value) => value;
    public static implicit operator ShaderObject((float x, float y, float z, float w) value) => value;

    public override string ToString()
        => Expression;
}