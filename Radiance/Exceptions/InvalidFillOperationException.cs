/* Author:  Leonardo Trevisan Silio
 * Date:    10/10/2024
 */
namespace Radiance.Exceptions;

public class InvalidFillOperationException : RadianceException
{
    public override string ErrorMessage =>
        """
        The data cannot be triangularizated. Consider use another function instead Utils.fill().
        """;
}