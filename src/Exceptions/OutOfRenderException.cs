/* Author:  Leonardo Trevisan Silio
 * Date:    12/09/2024
 */
namespace Radiance.Exceptions;

public class OutOfRenderException : RadianceException
{
    public override string ErrorMessage =>
        """
        A call of a render with all parameters outside the OnRender is ilegal.
        To make a curry with all paramters use .Curry(params) to avoid conflicts between curry action and call action.
        """;
}