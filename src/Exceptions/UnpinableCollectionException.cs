/* Author:  Leonardo Trevisan Silio
 * Date:    01/10/2024
 */
namespace Radiance.Exceptions;

public class UnpinableCollectionException : RadianceException
{
    public override string ErrorMessage =>
        $"""
        Is not possible pin a Render Collection with more than 0 registred calls.
        """;
}