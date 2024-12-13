/* Author:  Leonardo Trevisan Silio
 * Date:    13/12/2024
 */
namespace Radiance.Exceptions;

public class UnhandleableArgumentsException : RadianceException
{
    public override string ErrorMessage =>
        """
        The configured render cannot handle the sended parameters.
        """;
}