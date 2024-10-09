/* Author:  Leonardo Trevisan Silio
 * Date:    09/10/2024
 */
namespace Radiance.Exceptions;

public class InvalidFactoryDataException : RadianceException
{
    public override string ErrorMessage =>
        """
        The data used on renders cannot be handled by this chain factory.
        Configure The RenderParameterFactory.Chain if this was intended.
        Otherwise, check the type in the render call.
        """;
}