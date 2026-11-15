/* Author:  Leonardo Trevisan Silio
 * Date:    15/11/2026
 */
namespace Radiance.Exceptions;

public class ClipboardUnavailableException(string reason) : RadianceException
{
    public override string ErrorMessage =>
        $"""
        The system clipboard is in use by another application and could not be accessed: {reason}
        """;
}
