/* Author:  Leonardo Trevisan Silio
 * Date:    24/12/2024
 */
#pragma warning disable CS0660
#pragma warning disable CS0661
#pragma warning disable CS8981
#pragma warning disable IDE1006
#pragma warning disable IDE0130

using System.Globalization;
using System.Collections.Generic;

namespace Radiance;

using Shaders;

/// <summary>
/// Represent a Float data in shader implementation.
/// </summary>
public class val(
    string value,
    ShaderOrigin origin,
    IEnumerable<ShaderDependence> deps) 
    : ShaderObject(ShaderType.Float, value, origin, deps)
{
    public static implicit operator val(float value)
        => new (value.ToString(CultureInfo.InvariantCulture), ShaderOrigin.Global, []);
        
    public static implicit operator val(double value)
        => new (value.ToString(CultureInfo.InvariantCulture), ShaderOrigin.Global, []);
        
    public static implicit operator val(int value)
        => new (value.ToString(CultureInfo.InvariantCulture), ShaderOrigin.Global, []);
    
    public static boolean operator ==(val a, val b)
        => Union<boolean>($"({a} == {b})", a, b);
    
    public static boolean operator !=(val a, val b)
        => Union<boolean>($"({a} != {b})", a, b);

    public static boolean operator <(val a, val b)
        => Union<boolean>($"({a} < {b})", a, b);

    public static boolean operator >(val a, val b)
        => Union<boolean>($"({a} > {b})", a, b);
    
    public static boolean operator <=(val a, val b)
        => Union<boolean>($"({a} <= {b})", a, b);

    public static boolean operator >=(val a, val b)
        => Union<boolean>($"({a} >= {b})", a, b);

    public static val operator +(val a, val b)
        => Union<val>($"({a} + {b})", a, b);
    
    public static val operator -(val a, val b)
        => Union<val>($"({a} - {b})", a, b);
    
    public static val operator *(val a, val b)
        => Union<val>($"({a} * {b})", a, b);
    
    public static val operator /(val a, val b)
        => Union<val>($"({a} / {b})", a, b);

    public static val operator +(val a)
        => Transform<val, val>($"(+{a})", a);
    
    public static val operator -(val a)
        => Transform<val, val>($"(-{a})", a);
}