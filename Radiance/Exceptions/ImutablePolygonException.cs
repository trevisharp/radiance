/* Author:  Leonardo Trevisan Silio
 * Date:    27/09/2024
 */
using System;

namespace Radiance.Exceptions;

[Obsolete("Use ImmutablePolygonException.")]
public class ImutablePolygonException : RadianceException
{
    public override string ErrorMessage =>
        """
        A polygon cannot be modified.
        """;
}