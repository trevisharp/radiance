/* Author:  Leonardo Trevisan Silio
 * Date:    04/09/2024
 */
using System;

namespace Radiance.Exceptions;

public class ShaderOnlyResourceException : RadianceException
{
    public override string ErrorMessage =>
        """
        This resource only be called inside of a render.
        """;
}