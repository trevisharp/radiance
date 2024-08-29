/* Author:  Leonardo Trevisan Silio
 * Date:    01/02/2024
 */
using System;

namespace Radiance.Exceptions;

/// <summary>
/// Represents a error that occurs when a immutable polygon has modified.
/// </summary>
public class IlegalRenderMomentException : Exception
{
    public override string Message => 
        """
            You should not call a render outside a OnRender event.
        """;
}