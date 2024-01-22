/* Author:  Leonardo Trevisan Silio
 * Date:    19/01/2024
 */
using System;

namespace Radiance.Exceptions;

/// <summary>
/// Represents a error that occurs when a invalid parameters are used
/// in a render definition.
/// </summary>
public class InvalidRenderException : Exception
{
    public override string Message =>
    """
        The definion of the render use invalid type parameters.
        Try use Util.render function to simplify the definition.
    """;
}