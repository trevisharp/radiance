/* Author:  Leonardo Trevisan Silio
 * Date:    06/11/2024
 */
namespace Radiance.Exceptions;

public class InvalidImplementationException(object? obj) : RadianceException
{
    public override string ErrorMessage =>
        $"""
        '{obj?.ToString() ?? "null"}' is a invalid value for a implementation factory.
        """;
}