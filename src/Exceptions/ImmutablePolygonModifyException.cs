/* Author:  Leonardo Trevisan Silio
 * Date:    30/01/2024
 */
using System;

namespace Radiance.Exceptions;

/// <summary>
/// Represents a error that occurs when a immutable polygon has modified.
/// </summary>
public class ImmutablePolygonModifyException : Exception
{
    public override string Message => 
        """
            The window is not open now, so get window data is ilegal.
        """;
}