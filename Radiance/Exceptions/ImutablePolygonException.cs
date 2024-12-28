/* Author:  Leonardo Trevisan Silio
 * Date:    27/09/2024
 */
namespace Radiance.Exceptions;

public class ImutablePolygonException : RadianceException
{
    public override string ErrorMessage =>
        """
        A polygon cannot be modified.
        """;
}