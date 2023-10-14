/* Author:  Leonardo Trevisan Silio
 * Date:    14/10/2023
 */
#pragma warning disable CS8981

namespace Radiance.Types;

using Internal;
using ShaderSupport;
using ShaderSupport.Objects;

/// <summary>
/// Use lfloat to declare a local use of float with interoperability with shaders.
/// </summary>
public class lfloat : ShaderLocalReference<float, FloatShaderObject>
{
    public lfloat(string name, float value) 
        : base(name, value) { }
    
    public static implicit operator lfloat(float value)
        => new (ParamNamgeGenerator.GetNext(), value);
    
    public static lfloat operator ++(lfloat x)
        => new (x.Name, (float)x.Value + 1);
    
    public static lfloat operator --(lfloat x)
        => new (x.Name, (float)x.Value - 1);
    
    public static FloatShaderObject operator +(lfloat x, float y)
    {
        FloatShaderObject obj = x;
        return obj + y;
    }
    
    public static FloatShaderObject operator +(float y, lfloat x)
    {
        FloatShaderObject obj = x;
        return obj + y;
    }
    
    public static FloatShaderObject operator -(lfloat x, float y)
    {
        FloatShaderObject obj = x;
        return obj - y;
    }
    
    public static FloatShaderObject operator -(float y, lfloat x)
    {
        FloatShaderObject obj = x;
        return y - obj;
    }
        
    public static FloatShaderObject operator *(lfloat x, float y)
    {
        FloatShaderObject obj = x;
        return obj * y;
    }

    public static FloatShaderObject operator *(float y, lfloat x)
    {
        FloatShaderObject obj = x;
        return obj * y;
    }
        
    public static FloatShaderObject operator /(lfloat x, float y)
    {
        FloatShaderObject obj = x;
        return obj / y;
    }
    
    public static FloatShaderObject operator /(float y, lfloat x)
    {
        FloatShaderObject obj = x;
        return y / obj;
    }
}