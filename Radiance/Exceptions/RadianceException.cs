/* Author:  Leonardo Trevisan Silio
 * Date:    13/09/2024
 */
using System;

namespace Radiance.Exceptions;

public abstract class RadianceException : Exception
{
    public abstract string ErrorMessage { get; }

    public override string Message => ErrorMessage.ReplaceLineEndings(" ");
}