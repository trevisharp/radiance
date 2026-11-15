/* Author:  Leonardo Trevisan Silio
 * Date:    27/09/2024
 */
using System;

namespace Radiance.Exceptions;

#pragma warning disable CS0618
public class UnbufferedDataException : UnbufferedDataExcetion;
#pragma warning restore CS0618

[Obsolete("Use UnbufferedDataException.")]
public class UnbufferedDataExcetion : RadianceException
{
    public override string ErrorMessage =>
    """
    A BufferedData cannot be use wihtout a associated buffer.
    """;
}