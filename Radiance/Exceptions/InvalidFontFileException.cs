/* Author:  Leonardo Trevisan Silio
 * Date:    25/06/2025
 */
namespace Radiance.Exceptions;

public class InvalidFontFileException(string file) : RadianceException
{
    public override string ErrorMessage =>
        $"""
        Invalid extension type or missing font file '{file}'.
        """;
}