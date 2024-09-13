/* Author:  Leonardo Trevisan Silio
 * Date:    04/09/2024
 */
using System;

namespace Radiance.Exceptions;

public class WindowClosedException : RadianceException
{
    public override string ErrorMessage =>
        """
        Windows is closed and the deltatime cannot be readed.
        """;
}