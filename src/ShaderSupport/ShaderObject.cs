/* Author:  Leonardo Trevisan Silio
 * Date:    06/08/2023
 */
using System;

namespace Radiance.ShaderSupport;

using Objects;

/// <summary>
/// Represents any data in a shader implementation.
/// </summary>
public class ShaderObject
{
    public ShaderObject(
        ShaderType type,
        string name = null,
        string exp = null
    )
    {
        this.Type = type;
        this.Name = name;
        this.Expression = exp;
    }

    public string Name { get; private set; }
    public ShaderType Type { get; private set; }
    public string Expression { get; private set; }

    public string Value =>
        Name ?? Expression ?? "0";
    
    public static implicit operator ShaderObject(float value)
        => new FloatShaderObject(
            null,
            value.ToString()
        );
        
    public static implicit operator ShaderObject(double value)
        => new FloatShaderObject(
            null,
            value.ToString()
        );
        
    public static implicit operator ShaderObject(int value)
        => new FloatShaderObject(
            null,
            value.ToString()
        );
    
    public ShaderObject x
    {
        get => new ShaderObject(
            ShaderType.Float,
            null,
            $"({Value}).x"
        );
    }
    
    public ShaderObject y
    {
        get => new ShaderObject(
            ShaderType.Float,
            null,
            $"({Value}).y"
        );
    }
    
    public ShaderObject z
    {
        get => new ShaderObject(
            ShaderType.Float,
            null,
            $"({Value}).z"
        );
    }

    public static ShaderObject operator <(ShaderObject a, ShaderObject b)
    {
        return new ShaderObject(
            ShaderType.Bool,
            null,
            $"{a.Value} < {b.Value}"
        );
    }

    public static ShaderObject operator >(ShaderObject a, ShaderObject b)
    {
        return new ShaderObject(
            ShaderType.Bool,
            null,
            $"{a.Value} > {b.Value}"
        );
    }
    
    public static ShaderObject operator <=(ShaderObject a, ShaderObject b)
    {
        return new ShaderObject(
            ShaderType.Bool,
            null,
            $"{a.Value} <= {b.Value}"
        );
    }

    public static ShaderObject operator >=(ShaderObject a, ShaderObject b)
    {
        return new ShaderObject(
            ShaderType.Bool,
            null,
            $"{a.Value} >= {b.Value}"
        );
    }

    public static ShaderObject operator +(ShaderObject a, ShaderObject b)
    {
        return new ShaderObject(
            ShaderType.Bool,
            null,
            $"{a.Value} + {b.Value}"
        );
    }

    public static ShaderObject operator -(ShaderObject a, ShaderObject b)
    {
        return new ShaderObject(
            ShaderType.Bool,
            null,
            $"{a.Value} - {b.Value}"
        );
    }

    public static ShaderObject operator *(ShaderObject a, ShaderObject b)
    {
        return new ShaderObject(
            ShaderType.Bool,
            null,
            $"{a.Value} * {b.Value}"
        );
    }

    public static ShaderObject operator /(ShaderObject a, ShaderObject b)
    {
        return new ShaderObject(
            ShaderType.Bool,
            null,
            $"{a.Value} / {b.Value}"
        );
    }

    public static ShaderObject operator &(ShaderObject a, ShaderObject b)
    {
        if (a.Type != ShaderType.Bool && b.Type != ShaderType.Bool)
            throw new Exception("Both sides of & operator need be bool type.");
        
        return new ShaderObject(
            ShaderType.Bool,
            null,
            $"{a.Value} && {b.Value}"
        );
    }

    public static ShaderObject operator |(ShaderObject a, ShaderObject b)
    {
        if (a.Type != ShaderType.Bool | b.Type != ShaderType.Bool)
            throw new Exception("Both sides of | operator need be bool type.");
        
        return new ShaderObject(
            ShaderType.Bool,
            null,
            $"{a.Value} | {b.Value}"
        );
    }

    public static ShaderObject operator !(ShaderObject a)
    {
        if (a.Type != ShaderType.Bool)
            throw new Exception("! operator only can be applied in bool type.");
        
        return new ShaderObject(
            ShaderType.Bool,
            null,
            $"!{a.Value}"
        );
    }
}