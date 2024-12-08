/* Author:  Leonardo Trevisan Silio
 * Date:    17/10/2024
 */
namespace Radiance.Exceptions;

public class CallingNullArgumentException(object? prev, int index) : RadianceException
{
    readonly string argument = prev is null ? "first argument" : $"argument after {prev}";
    public override string ErrorMessage =>
        $"""
        On calling operation, the {argument} on index {index} is null.
        """;
}