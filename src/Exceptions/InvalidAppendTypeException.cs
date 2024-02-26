/* Author:  Leonardo Trevisan Silio
 * Date:    24/02/2024
 */
using System;

namespace Radiance.Exceptions;

/// <summary>
/// Represents a error that occurs when a append operation recive invalid types.
/// </summary>
public class InvalidAppendTypeException : Exception
{
    public override string Message => 
        """
            On append operation the only valid types are:
                float, Vec2, Vec3 and Vec4
        """;
}