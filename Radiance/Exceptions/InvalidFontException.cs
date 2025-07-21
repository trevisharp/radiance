/* Author:  Leonardo Trevisan Silio
 * Date:    21/07/2025
 */
namespace Radiance.Exceptions;

public class InvalidFontException(string file, string reason) : RadianceException
{
    public override string ErrorMessage =>
        $"""
        The file {file} is not a valid font file.
        Reason: {reason}
        """;
}