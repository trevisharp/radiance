/* Author:  Leonardo Trevisan Silio
 * Date:    21/08/2023
 */
namespace Radiance.ShaderSupport;

using Dependencies;

public class ShaderOutput
{
    public static ShaderOutput Create<T>(T obj)
        where T : ShaderObject, new()
    {
        ShaderOutput output = new ShaderOutput();

        output.Value = obj;
        output.Dependence = new VariableDependence<T>();

        return output;
    }

    public ShaderObject Value { get; set; }
    public ShaderDependence Dependence { get; set; }

    private static ShaderOutput[] empty = new ShaderOutput[0];
    public static ShaderOutput[] Empty => Empty;
}