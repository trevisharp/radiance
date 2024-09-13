/* Author:  Leonardo Trevisan Silio
 * Date:    12/09/2024
 */
using System;

namespace Radiance.Exceptions;

public class OutOfRenderException : Exception
{
    public override string Message => 
        """
        A call of a render with all parameters outside the OnRender is ilegal.
        To make a curry with all paramters use .Curry(params) to avoid conflicts between curry action and call action.
        """;
}