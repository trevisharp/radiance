/* Author:  Leonardo Trevisan Silio
 * Date:    19/01/2024
 */
using System;

namespace Radiance.Exceptions;

/// <summary>
/// Represents a error that occurs when a render is called with missing parameters.
/// </summary>
public class MissingParametersException : Exception
{
    public override string Message =>
    """
    A render call with missing parameters throw a exception.
    Add missing parameters to solve this exception.
    """;
}