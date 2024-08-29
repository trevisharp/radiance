/* Author:  Leonardo Trevisan Silio
 * Date:    19/02/2024
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
            Try get values has width and heigth of screen after the Window.Open call.
            You can do this moving code to Window Load event.
        """;
}