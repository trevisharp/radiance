/* Author:  Leonardo Trevisan Silio
 * Date:    14/10/2023
 */
#pragma warning disable CS8981

using System.Reflection;

namespace Radiance.Types;

using Shaders;
using Shaders.Objects;

/// <summary>
/// Use gfloat to declare a global use of float with interoperability with shaders.
/// </summary>
public class gfloat : ShaderGlobalReference<float, FloatShaderObject>
{
    public gfloat(FieldInfo field, object baseObject, object value) 
        : base(field, baseObject, value) { }
    
    public static implicit operator gfloat(float value)
        => new gfloat(null, null, value);
    
    public static gfloat operator ++(gfloat x)
        => new (x.Field, x.BaseObject, x.Value + 1);
    
    public static gfloat operator --(gfloat x)
        => new (x.Field, x.BaseObject, x.Value - 1);
    
    public static gfloat operator +(gfloat x, float y)
        => new (x.Field, x.BaseObject, x.Value + y);
    
    public static gfloat operator +(float y, gfloat x)
        => new (x.Field, x.BaseObject, x.Value + y);
    
    public static gfloat operator -(gfloat x, float y)
        => new (x.Field, x.BaseObject, x.Value - y);
    
    public static gfloat operator -(float y, gfloat x)
        => new (x.Field, x.BaseObject, y - x.Value);
        
    public static gfloat operator *(gfloat x, float y)
        => new (x.Field, x.BaseObject, x.Value * y);

    public static gfloat operator *(float y, gfloat x)
        => new (x.Field, x.BaseObject, x.Value * y);
        
    public static gfloat operator /(gfloat x, float y)
        => new (x.Field, x.BaseObject, x.Value / y);
    
    public static gfloat operator /(float y, gfloat x)
        => new (x.Field, x.BaseObject, x.Value / y);
}