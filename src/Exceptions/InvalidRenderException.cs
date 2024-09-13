/* Author:  Leonardo Trevisan Silio
 * Date:    04/09/2024
 */
using System;

namespace Radiance.Exceptions;

public class InvalidRenderException : RadianceException
{
    public override string ErrorMessage =>
        """
        All parameters of a Render Delegate need be from type FloatShaderObject or TextureShaderObject.
        """;
}