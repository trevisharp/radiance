/* Author:  Leonardo Trevisan Silio
 * Date:    25/09/2024
 */
namespace Radiance.Exceptions;

public class BuilderEmptyException(string prop) : RadianceException
{
    public override string ErrorMessage =>
        $"""
        The {prop} is null and is required to generate shader pipeline.  
        """;
}