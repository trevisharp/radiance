/* Author:  Leonardo Trevisan Silio
 * Date:    04/09/2024
 */
using System;

namespace Radiance.Exceptions;

public class ShaderOnlyResourceException : Exception
{
    public override string Message => 
        """
            This resource only be called inside of a render.
        """;
}