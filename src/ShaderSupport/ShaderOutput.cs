/* Author:  Leonardo Trevisan Silio
 * Date:    21/08/2023
 */
namespace Radiance.ShaderSupport;

using Dependencies;

/// <summary>
/// Represents a output from Vertex Shader to Fragment Shader
/// </summary>
public abstract class ShaderOutput
{
    public static ShaderOutput<T> Create<T>(T obj)
    where T : ShaderObject, new()
    {
        var output = new ShaderOutput<T>();

        output.Value = obj;
        output.Dependence = new VariableDependence<T>();

        return output;
    }
    
    public abstract ShaderObject BaseValue { get; }
    public abstract ShaderDependence BaseDependence { get; }

    private static ShaderOutput[] empty = new ShaderOutput[0];
    public static ShaderOutput[] Empty => empty;
}

/// <summary>
/// Represents a output from Vertex Shader to Fragment Shader
/// </summary>
public class ShaderOutput<T> : ShaderOutput
    where T : ShaderObject, new()
{
    public T Value { get; set; }
    public VariableDependence<T> Dependence { get; set; }

    public override ShaderObject BaseValue => Value;
    public override ShaderDependence BaseDependence => Dependence;
}