/* Author:  Leonardo Trevisan Silio
 * Date:    17/09/2024
 */
namespace Radiance.Exceptions;

public class InvalidShaderExpressionException(object? obj) : RadianceException
{
    public override string ErrorMessage =>
        $"""
        The object '{obj}' cannot be converted to a Shader Expression.
        """;
}