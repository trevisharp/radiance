/* Author:  Leonardo Trevisan Silio
 * Date:    04/12/2024
 */
using Radiance.Primitives;

namespace Radiance.Exceptions;

public class InvalidUniformTypeException(object obj, string expectedType) : RadianceException
{
    public override string ErrorMessage =>
        $"""
        The value of type '{obj.GetType()}' received on Uniform update is not from expected type '{expectedType}'.
        {(obj is SkipCurryingParameter ? "This can may occurs when a render is called with a missing value." : "")}
        """;
}