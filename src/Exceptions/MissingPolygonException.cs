/* Author:  Leonardo Trevisan Silio
 * Date:    04/09/2024
 */
using System;

namespace Radiance.Exceptions;

public class MissingPolygonException : Exception
{
    public override string Message => 
        """
            A render need be called with a polygon as first argument.
        """;
}