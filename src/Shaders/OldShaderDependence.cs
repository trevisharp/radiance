/* Author:  Leonardo Trevisan Silio
 * Date:    23/01/2024
 */
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Radiance.Shaders;

using Exceptions;

/// <summary>
/// Represents a Input for a Shader Implementation.
/// </summary>
public abstract class OldShaderDependence
{
    public string Name { get; set; }
    public ShaderDependenceType DependenceType { get; set; }
    public abstract object Value { get; }
    public abstract string GetHeader();
    public virtual string GetCode() => null;
    public virtual void UpdateValue(object newValue)
        => throw new ReadonlyDependenceException();
    public static ShaderDependenceComparer Comparer
        => new ShaderDependenceComparer();
}

public abstract class ShaderDependence<T> : OldShaderDependence
    where T : ShaderObject, new()
{
    public static implicit operator T(ShaderDependence<T> dependece)
        => new()
        {
            Expression = dependece.Name,
            Dependecies = new OldShaderDependence[] { dependece }
        };
}

public class ShaderDependenceComparer : IEqualityComparer<OldShaderDependence>
{
    public bool Equals(OldShaderDependence x, OldShaderDependence y)
        => x.GetHeader() == y.GetHeader();
    
    public int GetHashCode([DisallowNull] OldShaderDependence obj)
        => obj.GetHeader().GetHashCode();
}