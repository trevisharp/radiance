/* Author:  Leonardo Trevisan Silio
 * Date:    21/08/2023
 */
namespace Radiance.ShaderSupport;

public class ShaderOutput
{
    public ShaderObject Value { get; set; }
    public ShaderDependence Dependence { get; set; }

    private static ShaderOutput[] empty = new ShaderOutput[0];
    public static ShaderOutput[] Empty => Empty;
}