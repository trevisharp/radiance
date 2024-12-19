/* Author:  Leonardo Trevisan Silio
 * Date:    13/12/2024
 */
using System;

namespace Radiance.Exceptions;

public class UnhandleableArgumentsException(Type type) : RadianceException
{
    public override string ErrorMessage =>
        $"""
        A render cannot have a parameter with the type '{type}'.
        """;
}