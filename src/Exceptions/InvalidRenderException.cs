/* Author:  Leonardo Trevisan Silio
 * Date:    17/09/2024
 */
using System.Reflection;

namespace Radiance.Exceptions;

public class InvalidRenderException(ParameterInfo parameter) : RadianceException
{
    public override string ErrorMessage =>
        $"""
        The parameter {parameter.Name} has a invalid type {parameter.ParameterType} or cannot be curryied.
        """;
}