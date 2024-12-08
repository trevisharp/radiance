/* Author:  Leonardo Trevisan Silio
 * Date:    17/09/2024
 */
using System.Reflection;

namespace Radiance.Exceptions;

public class InvalidRenderException(ParameterInfo parameter) : RadianceException
{
    public override string ErrorMessage =>
        $"""
        The parameter {parameter.Name} with type {parameter.ParameterType} recive a invalid value or cannot be curryied.
        """;
}