/* Author:  Leonardo Trevisan Silio
 * Date:    04/09/2024
 */
using System;

namespace Radiance.Exceptions;

public class WindowClosedException : RadianceException
{
    public override string ErrorMessage =>
        """
        Windows is closed and properties like deltatime and size cannot be readed.
        """;
}