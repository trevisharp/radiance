/* Author:  Leonardo Trevisan Silio
 * Date:    15/08/2023
 */
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Radiance.Shaders;

/// <summary>
/// Represents a Input for a Shader Implementation.
/// </summary>
public abstract class ShaderDependence
{
    public abstract object Value { get; }
    public abstract string GetHeader();
    public string Name { get; set; }
    public ShaderDependenceType DependenceType { get; set; }

    public static ShaderDependenceComparer Comparer
        => new ShaderDependenceComparer();
}

public abstract class ShaderDependence<T> : ShaderDependence
    where T : ShaderObject, new()
{
    public static implicit operator T(ShaderDependence<T> dependece)
    {
        T obj = new();

        obj.Expression = dependece.Name;
        obj.Dependecies = new ShaderDependence[] { dependece };

        return obj;
    }
}

public class ShaderDependenceComparer : IEqualityComparer<ShaderDependence>
{
    public bool Equals(ShaderDependence x, ShaderDependence y)
        => x.GetHeader() == y.GetHeader();
    
    public int GetHashCode([DisallowNull] ShaderDependence obj)
        => obj.GetHeader().GetHashCode();
}