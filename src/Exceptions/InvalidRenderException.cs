/* Author:  Leonardo Trevisan Silio
 * Date:    04/09/2024
 */
using System;

namespace Radiance.Exceptions;

public class InvalidRenderException : Exception
{
    public override string Message => 
        """
            All parameters of a Render Delegate need be from type FloatShaderObject or TextureShaderObject.
        """;
}