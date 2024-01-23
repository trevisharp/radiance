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
public abstract class ShaderDependence
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

public abstract class ShaderDependence<T> : ShaderDependence
    where T : ShaderObject, new()
{
    public static implicit operator T(ShaderDependence<T> dependece)
        => new()
        {
            Expression = dependece.Name,
            Dependecies = new ShaderDependence[] { dependece }
        };
}

public class ShaderDependenceComparer : IEqualityComparer<ShaderDependence>
{
    public bool Equals(ShaderDependence x, ShaderDependence y)
        => x.GetHeader() == y.GetHeader();
    
    public int GetHashCode([DisallowNull] ShaderDependence obj)
        => obj.GetHeader().GetHashCode();
}