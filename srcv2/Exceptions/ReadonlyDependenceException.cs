/* Author:  Leonardo Trevisan Silio
 * Date:    22/01/2024
 */
using System;

namespace Radiance.Exceptions;

/// <summary>
/// Represents a error that occurs when a readonly dependece value has updated.
/// </summary>
public class ReadonlyDependenceException : Exception
{
    public override string Message =>
    """
    A readonly dependence (like width, height, time) suffered an
    ilegal attempt to manually modify its data.
    """;
}