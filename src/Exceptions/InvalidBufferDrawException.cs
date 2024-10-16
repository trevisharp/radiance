/* Author:  Leonardo Trevisan Silio
 * Date:    16/10/2024
 */
namespace Radiance.Exceptions;

public class InvalidBufferDrawException : RadianceException
{
    public override string ErrorMessage =>
        """
        For call a render that use draw() function inside it, you
        need to use a Polygon buffer parameter instead another
        buffer type.
        """;
}