/* Author:  Leonardo Trevisan Silio
 * Date:    30/01/2024
 */
using System;

namespace Radiance.Exceptions;

/// <summary>
/// Represents a error that occurs when a window of data is acessed with Window closed.
/// </summary>
public class WindowClosedException : Exception
{
    public override string Message => 
        """
            The window is not open now, so get window data is ilegal.
        """;
}