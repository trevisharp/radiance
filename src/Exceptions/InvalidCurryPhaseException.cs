/* Author:  Leonardo Trevisan Silio
 * Date:    16/09/2024
 */
namespace Radiance.Exceptions;

public class InvalidCurryPhaseException : RadianceException
{
    public override string ErrorMessage => 
    """
    A currying operation, when we call a render with less parameters than needed to fix this parameters
    is forbiden inside the OnRender Windown event.
    This error may occur when in render calling and we missing a parameter.
    """;
}