/* Author:  Leonardo Trevisan Silio
 * Date:    19/01/2024
 */
using System;

namespace Radiance.Exceptions;

/// <summary>
/// Represents a error that occurs when a render is called with missing polygon parameter.
/// </summary>
public class MissingPolygonException : Exception
{
    public override string Message =>
    """
    A render call with missing polygon parameter throw a exception.
    Every render should me called with a polygon has first parameter.
    """;
}