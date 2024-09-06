/* Author:  Leonardo Trevisan Silio
 * Date:    04/09/2024
 */
using System;

namespace Radiance.Exceptions;

public class WindowClosedException : Exception
{
    public override string Message =>
        """
        Windows is closed and the deltatime cannot be readed.
        """;
}