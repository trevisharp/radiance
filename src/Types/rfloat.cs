/* Author:  Leonardo Trevisan Silio
 * Date:    28/08/2023
 */
using System.Reflection;

namespace Radiance.Types;

using ShaderSupport;
using ShaderSupport.Objects;

public class rfloat : ShaderReference<float, FloatShaderObject>
{
    public rfloat(float value)
        : base(value) { }

    public rfloat(FieldInfo field, object baseObject) 
        : base(field, baseObject) { }
    
    public static implicit operator rfloat(float value)
        => new rfloat(value);
    
    public static rfloat operator +(rfloat x, float y)
        => new ((float)x.Value + y);
    
    public static rfloat operator +(float y, rfloat x)
        => new ((float)x.Value + y);
    
    public static rfloat operator -(rfloat x, float y)
        => new ((float)x.Value - y);
    
    public static rfloat operator -(float y, rfloat x)
        => new (y - (float)x.Value);
        
    public static rfloat operator *(rfloat x, float y)
        => new ((float)x.Value * y);
    
    public static rfloat operator *(float y, rfloat x)
        => new ((float)x.Value * y);
        
    public static rfloat operator /(rfloat x, float y)
        => new ((float)x.Value / y);
    
    public static rfloat operator /(float y, rfloat x)
        => new (y / (float)x.Value);
}