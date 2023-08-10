/* Author:  Leonardo Trevisan Silio
 * Date:    10/08/2023
 */
using System.Linq;
using System.Collections.Generic; 

namespace Radiance.ShaderSupport.Objects;

/// <summary>
/// Represent a Float data in shader implementation.
/// </summary>
public class FloatShaderObject : ShaderObject
{
    public FloatShaderObject()
    {
        this.Expression = "0.0";
        this.Dependecies = new ShaderDependence[0];
        this.Type = ShaderType.Float;
    }

    public FloatShaderObject(string value, params ShaderDependence[] dependecies)
    {
        this.Expression = value;
        this.Dependecies = dependecies;
        this.Type = ShaderType.Float;
    }

    public FloatShaderObject(string value, IEnumerable<ShaderDependence> dependecies)
    {
        this.Expression = value;
        this.Dependecies = dependecies;
        this.Type = ShaderType.Float;
    }

    public static implicit operator FloatShaderObject(float value)
        => new (value.ToString());
        
    public static implicit operator FloatShaderObject(double value)
        => new (value.ToString());
        
    public static implicit operator FloatShaderObject(int value)
        => new (value.ToString());
    
    public static BoolShaderObject operator ==(FloatShaderObject a, FloatShaderObject b)
    {
        return new BoolShaderObject(
            $"({a.Expression}) == ({b.Expression})",
            a.Dependecies.Concat(b.Dependecies)
        );
    }
    
    public static BoolShaderObject operator !=(FloatShaderObject a, FloatShaderObject b)
    {
        return new BoolShaderObject(
            $"({a.Expression}) != ({b.Expression})",
            a.Dependecies.Concat(b.Dependecies)
        );
    }

    public static BoolShaderObject operator <(FloatShaderObject a, FloatShaderObject b)
    {
        return new BoolShaderObject(
            $"({a.Expression}) < ({b.Expression})",
            a.Dependecies.Concat(b.Dependecies)
        );
    }

    public static BoolShaderObject operator >(FloatShaderObject a, FloatShaderObject b)
    {
        return new BoolShaderObject(
            $"({a.Expression}) > ({b.Expression})",
            a.Dependecies.Concat(b.Dependecies)
        );
    }
    
    public static BoolShaderObject operator <=(FloatShaderObject a, FloatShaderObject b)
    {
        return new BoolShaderObject(
            $"({a.Expression}) <= ({b.Expression})",
            a.Dependecies.Concat(b.Dependecies)
        );
    }

    public static BoolShaderObject operator >=(FloatShaderObject a, FloatShaderObject b)
    {
        return new BoolShaderObject(
            $"({a.Expression}) >= ({b.Expression})",
            a.Dependecies.Concat(b.Dependecies)
        );
    }

    public static FloatShaderObject operator +(FloatShaderObject x, FloatShaderObject y)
    {
        var dependecies = x.Dependecies.Concat(y.Dependecies);
        return new ($"({x}) + ({y})", dependecies);
    }
    
    public static FloatShaderObject operator -(FloatShaderObject x, FloatShaderObject y)
    {
        var dependecies = x.Dependecies.Concat(y.Dependecies);
        return new ($"({x}) - ({y})", dependecies);
    }
    
    public static FloatShaderObject operator *(FloatShaderObject x, FloatShaderObject y)
    {
        var dependecies = x.Dependecies.Concat(y.Dependecies);
        return new ($"({x}) * ({y})", dependecies);
    }
    
    public static FloatShaderObject operator /(FloatShaderObject x, FloatShaderObject y)
    {
        var dependecies = x.Dependecies.Concat(y.Dependecies);
        return new ($"({x}) / ({y})", dependecies);
    }

    public static FloatShaderObject operator +(FloatShaderObject x)
    {
        var dependecies = x.Dependecies;
        return new ($"+({x})", dependecies);
    }
    
    public static FloatShaderObject operator -(FloatShaderObject x)
    {
        var dependecies = x.Dependecies;
        return new ($"-({x})", dependecies);
    }
}