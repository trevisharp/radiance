/* Author:  Leonardo Trevisan Silio
 * Date:    25/09/2024
 */
namespace Radiance.Exceptions;

public class UncreatedProgramException : RadianceException
{
    public override string ErrorMessage =>
        $"""
        The program associeated with the Render Context are not created yet.
        """;
}