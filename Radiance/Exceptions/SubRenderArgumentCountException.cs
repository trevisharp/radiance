/* Author:  Leonardo Trevisan Silio
 * Date:    19/09/2024
 */
namespace Radiance.Exceptions;

public class SubRenderArgumentCountException(int expected, int recived) : RadianceException
{
    public override string ErrorMessage =>
        $"""
        A render expected {expected} parameters, but recived {recived} arguments.
        Remember: A render called inside another render uses the polygon of
        parent call so it needs one parameter less.
        """;
}