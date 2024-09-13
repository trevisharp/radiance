/* Author:  Leonardo Trevisan Silio
 * Date:    04/09/2024
 */
using System;

namespace Radiance.Exceptions;

public class ExcessOfArgumentsException : RadianceException
{
    public override string ErrorMessage =>
        """
        A render is called with more arguments than parameters.
        Remember that a type like Vec3 counts as 3 arguments.
        """;
}