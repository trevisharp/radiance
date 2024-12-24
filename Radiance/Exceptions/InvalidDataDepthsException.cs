/* Author:  Leonardo Trevisan Silio
 * Date:    24/12/2024
 */
namespace Radiance.Exceptions;

public class InvalidDataDepthsException(object value, int depth, int otherDepth) : RadianceException
{
    public override string ErrorMessage =>
        $"""
        A render need recive all buffer values with the same number of rows but,
        recive a value of type {value.GetType()} with depth {depth}, but another
        buffer already has depth {otherDepth}.
        """;
}