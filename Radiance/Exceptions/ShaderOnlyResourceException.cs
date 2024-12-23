/* Author:  Leonardo Trevisan Silio
 * Date:    04/09/2024
 */
namespace Radiance.Exceptions;

public class ShaderOnlyResourceException(string name) : RadianceException
{
    public override string ErrorMessage =>
        $"""
        The resource {name} only be called inside of a render.
        """;
}