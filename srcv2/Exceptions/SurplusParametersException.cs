/* Author:  Leonardo Trevisan Silio
 * Date:    19/01/2024
 */
using System;

namespace Radiance.Exceptions;

/// <summary>
/// Represents a error that occurs when a render is called with surplus parameters.
/// </summary>
public class SurplusParametersException : Exception
{
    public override string Message =>
    """
    A render call with surplus parameters throw a exception.
    Remove surplus parameters to solve this exception.
    """;
}