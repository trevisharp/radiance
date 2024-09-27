/* Author:  Leonardo Trevisan Silio
 * Date:    27/09/2024
 */
namespace Radiance.Exceptions;

public class UnbufferedDataExcetion : RadianceException
{
    public override string ErrorMessage =>
    """
    A BufferedData cannot be use wihtout a associated buffer.
    """;
}