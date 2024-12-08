/* Author:  Leonardo Trevisan Silio
 * Date:    02/10/2024
 */
namespace Radiance.Exceptions;

public class MissingPolygonException : RadianceException
{
    public override string ErrorMessage =>
        """
        A render need be called with a polygon or other bufferd data as first argument.
        """;
}