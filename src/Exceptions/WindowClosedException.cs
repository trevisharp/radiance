/* Author:  Leonardo Trevisan Silio
 * Date:    30/01/2024
 */
using System;

namespace Radiance.Exceptions;

public class WindowClosedException : Exception
{
    public override string Message => 
        """
            Polygon type can call MakeImmutable() method making the type immutable.
            A immutable polygon cannot be modified, therefore, it cannot perform the 
            Add and Append methods.
        """;
}