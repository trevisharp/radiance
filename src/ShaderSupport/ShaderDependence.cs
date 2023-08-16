/* Author:  Leonardo Trevisan Silio
 * Date:    15/08/2023
 */
namespace Radiance.ShaderSupport;

/// <summary>
/// Represents a Input for a Shader Implementation.
/// </summary>
public abstract class ShaderDependence
{
    public abstract object Value { get; }
    public abstract string GetHeader();
    public string Name { get; set; }
    public ShaderDependenceType DependenceType { get; set; }
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