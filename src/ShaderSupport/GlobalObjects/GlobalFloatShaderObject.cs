/* Author:  Leonardo Trevisan Silio
 * Date:    22/08/2023
 */
using System.Reflection;

namespace Radiance.ShaderSupport.GlobalObjects;

using Objects;

public class GlobalFloatShaderObject : GlobalShaderObject<float, FloatShaderObject>
{
    public GlobalFloatShaderObject(float value)
        : base(value) { }

    public GlobalFloatShaderObject(FieldInfo field, object baseObject) 
        : base(field, baseObject) { }
    
    public static implicit operator GlobalFloatShaderObject(float value)
        => new GlobalFloatShaderObject(value);
    
    public static GlobalFloatShaderObject operator +(GlobalFloatShaderObject x, float y)
        => new ((float)x.Value + y);
    
    public static GlobalFloatShaderObject operator +(float y, GlobalFloatShaderObject x)
        => new ((float)x.Value + y);
    
    public static GlobalFloatShaderObject operator -(GlobalFloatShaderObject x, float y)
        => new ((float)x.Value - y);
    
    public static GlobalFloatShaderObject operator -(float y, GlobalFloatShaderObject x)
        => new (y - (float)x.Value);
        
    public static GlobalFloatShaderObject operator *(GlobalFloatShaderObject x, float y)
        => new ((float)x.Value * y);
    
    public static GlobalFloatShaderObject operator *(float y, GlobalFloatShaderObject x)
        => new ((float)x.Value * y);
        
    public static GlobalFloatShaderObject operator /(GlobalFloatShaderObject x, float y)
        => new ((float)x.Value / y);
    
    public static GlobalFloatShaderObject operator /(float y, GlobalFloatShaderObject x)
        => new (y / (float)x.Value);
}